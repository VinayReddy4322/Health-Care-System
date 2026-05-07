using DoctorService.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorService.Data
{
    public class DoctorDbContext : DbContext
    {
        public DoctorDbContext(DbContextOptions<DoctorDbContext> options) : base(options) { }

        public DbSet<Doctor> Doctors { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>()
                .HasIndex(doctor => doctor.UserId)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .Property(doctor => doctor.Name)
                .HasMaxLength(120);

            modelBuilder.Entity<Doctor>()
                .Property(doctor => doctor.Specialization)
                .HasMaxLength(120);
        }
    }
}
