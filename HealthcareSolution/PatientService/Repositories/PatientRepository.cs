using Microsoft.EntityFrameworkCore;
using PatientService.Data;
using PatientService.Models;

namespace PatientService.Repositories
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> GetByUserIdAsync(int userId);
        Task<Patient> AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(int id);
    }

    public class PatientRepository : IPatientRepository
    {
        private readonly PatientDbContext _db;
        public PatientRepository(PatientDbContext db) => _db = db;

        public async Task<List<Patient>> GetAllAsync() => await _db.Patients.ToListAsync();

        public async Task<Patient?> GetByIdAsync(int id) => await _db.Patients.FindAsync(id);

        public async Task<Patient?> GetByUserIdAsync(int userId) =>
            await _db.Patients.FirstOrDefaultAsync(patient => patient.UserId == userId);

        public async Task<Patient> AddAsync(Patient patient)
        {
            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();
            return patient;
        }

        public async Task UpdateAsync(Patient patient)
        {
            _db.Patients.Update(patient);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p != null)
            {
                _db.Patients.Remove(p);
                await _db.SaveChangesAsync();
            }
        }
    }
}
