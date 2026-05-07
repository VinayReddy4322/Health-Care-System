using System.ComponentModel.DataAnnotations;

namespace DoctorService.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Specialization { get; set; } = string.Empty;
    }
}
