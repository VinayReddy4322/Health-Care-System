using AppointmentService.Models;
using AppointmentService.Repositories;

namespace AppointmentService.Services
{
    public interface IAppointmentService
    {
        Task<Appointment> BookAsync(Appointment appointment);
        Task<bool> CancelAsync(int appointmentId, string requesterRole, int requesterId);
        Task<bool> CompleteAsync(int appointmentId, string requesterRole, int requesterId);
        Task<List<Appointment>> GetForUserAsync(string requesterRole, int requesterId);
        Task<Appointment?> GetByIdAsync(int id);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly IHttpClientFactory _httpClientFactory;

        public AppointmentService(IAppointmentRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _httpClientFactory = httpClientFactory;
        }

        public Task<Appointment?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public async Task<List<Appointment>> GetForUserAsync(string requesterRole, int requesterId)
        {
            var all = await _repo.GetAllAsync();
            return requesterRole switch
            {
                "Admin" => all,
                "Doctor" => all.Where(appointment => appointment.DoctorId == requesterId).ToList(),
                "Patient" => all.Where(appointment => appointment.PatientId == requesterId).ToList(),
                _ => []
            };
        }

        public async Task<Appointment> BookAsync(Appointment appointment)
        {
            if (appointment.AppointmentDate <= DateTime.UtcNow)
                throw new InvalidOperationException("Appointment date must be in the future");

            if (!await DoctorExistsAsync(appointment.DoctorId))
                throw new InvalidOperationException("Doctor does not exist");

            appointment.Status = AppointmentStatus.Booked;
            return await _repo.AddAsync(appointment);
        }

        public async Task<bool> CancelAsync(int appointmentId, string requesterRole, int requesterId)
        {
            var appointment = await _repo.GetByIdAsync(appointmentId);
            if (appointment == null) return false;

            if (appointment.Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed appointment");

            if (requesterRole == "Patient" && appointment.PatientId != requesterId)
                throw new UnauthorizedAccessException();

            if (requesterRole == "Doctor" && appointment.DoctorId != requesterId)
                throw new UnauthorizedAccessException();

            appointment.Status = AppointmentStatus.Cancelled;
            await _repo.UpdateAsync(appointment);
            return true;
        }

        public async Task<bool> CompleteAsync(int appointmentId, string requesterRole, int requesterId)
        {
            var appointment = await _repo.GetByIdAsync(appointmentId);
            if (appointment == null) return false;

            if (appointment.Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Cannot complete a cancelled appointment");

            if (appointment.Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Appointment is already completed");

            if (requesterRole == "Doctor" && appointment.DoctorId != requesterId)
                throw new UnauthorizedAccessException();

            appointment.Status = AppointmentStatus.Completed;
            await _repo.UpdateAsync(appointment);
            return true;
        }

        private async Task<bool> DoctorExistsAsync(int doctorId)
        {
            var client = _httpClientFactory.CreateClient("DoctorService");
            try
            {
                using var response = await client.GetAsync($"/api/doctors/by-user/{doctorId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
