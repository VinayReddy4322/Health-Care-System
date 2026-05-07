using System.Collections.Generic;
using System.Threading.Tasks;
using DoctorService.Models;
using DoctorService.Repositories;

namespace DoctorService.Services
{
    public interface IDoctorService
    {
        Task<List<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(int id);
        Task<Doctor?> GetByUserIdAsync(int userId);
        Task<Doctor> CreateAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(int id);
    }

    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repo;
        public DoctorService(IDoctorRepository repo) => _repo = repo;

        public Task<List<Doctor>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Doctor?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task<Doctor?> GetByUserIdAsync(int userId) => _repo.GetByUserIdAsync(userId);

        public Task<Doctor> CreateAsync(Doctor doctor) => _repo.AddAsync(doctor);

        public Task UpdateAsync(Doctor doctor) => _repo.UpdateAsync(doctor);

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
