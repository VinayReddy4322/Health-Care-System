using System.Collections.Generic;
using System.Threading.Tasks;
using DoctorService.Data;
using DoctorService.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorService.Repositories
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(int id);
        Task<Doctor?> GetByUserIdAsync(int userId);
        Task<Doctor> AddAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(int id);
    }

    public class DoctorRepository : IDoctorRepository
    {
        private readonly DoctorDbContext _db;
        public DoctorRepository(DoctorDbContext db) => _db = db;

        public async Task<List<Doctor>> GetAllAsync() => await _db.Doctors.ToListAsync();

        public async Task<Doctor?> GetByIdAsync(int id) => await _db.Doctors.FindAsync(id);

        public async Task<Doctor?> GetByUserIdAsync(int userId) =>
            await _db.Doctors.FirstOrDefaultAsync(doctor => doctor.UserId == userId);

        public async Task<Doctor> AddAsync(Doctor doctor)
        {
            _db.Doctors.Add(doctor);
            await _db.SaveChangesAsync();
            return doctor;
        }

        public async Task UpdateAsync(Doctor doctor)
        {
            _db.Doctors.Update(doctor);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var d = await _db.Doctors.FindAsync(id);
            if (d != null)
            {
                _db.Doctors.Remove(d);
                await _db.SaveChangesAsync();
            }
        }
    }
}
