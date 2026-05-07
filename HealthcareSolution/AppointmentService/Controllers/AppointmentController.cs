using AppointmentService.Models;
using AppointmentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppointmentService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;
        public AppointmentController(IAppointmentService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetForUserAsync(CurrentRole(), CurrentUserId());
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var appointment = await _service.GetByIdAsync(id);
            if (appointment == null) return NotFound();
            if (!CanRead(appointment)) return Forbid();
            return Ok(appointment);
        }

        [Authorize(Roles = "Patient,Admin")]
        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] Appointment appointment)
        {
            if (User.IsInRole("Patient")) appointment.PatientId = CurrentUserId();

            try
            {
                var created = await _service.BookAsync(appointment);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Patient,Doctor,Admin")]
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var ok = await _service.CancelAsync(id, CurrentRole(), CurrentUserId());
                return ok ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpPost("{id:int}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                var ok = await _service.CompleteAsync(id, CurrentRole(), CurrentUserId());
                return ok ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool CanRead(Appointment appointment) =>
            User.IsInRole("Admin") ||
            (User.IsInRole("Patient") && appointment.PatientId == CurrentUserId()) ||
            (User.IsInRole("Doctor") && appointment.DoctorId == CurrentUserId());

        private int CurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        private string CurrentRole() => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
}
