using PatientService.Models;
using PatientService.Repositories;

namespace PatientService.Services
{
    public interface IPatientService
    {
        Task<List<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> GetByUserIdAsync(int userId);
        Task<Patient> CreateAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(int id);
    }

    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repo;
        public PatientService(IPatientRepository repo) => _repo = repo;

        public Task<List<Patient>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Patient?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task<Patient?> GetByUserIdAsync(int userId) => _repo.GetByUserIdAsync(userId);

        public Task<Patient> CreateAsync(Patient patient) => _repo.AddAsync(patient);

        public Task UpdateAsync(Patient patient) => _repo.UpdateAsync(patient);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
