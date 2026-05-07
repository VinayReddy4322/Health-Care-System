using AppointmentService.Data;
using AppointmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Repositories
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
    }

    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppointmentDbContext _db;
        public AppointmentRepository(AppointmentDbContext db) => _db = db;

        public async Task<List<Appointment>> GetAllAsync() => await _db.Appointments.ToListAsync();

        public async Task<Appointment?> GetByIdAsync(int id) => await _db.Appointments.FindAsync(id);

        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();
            return appointment;
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
        }
    }
}
