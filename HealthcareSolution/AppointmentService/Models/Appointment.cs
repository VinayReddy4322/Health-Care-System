using System.ComponentModel.DataAnnotations;

namespace AppointmentService.Models
{
    public enum AppointmentStatus
    {
        Booked = 1,
        Cancelled = 2,
        Completed = 3
    }

    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;
    }
}
