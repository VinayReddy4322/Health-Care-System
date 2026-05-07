using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientService.Models;
using PatientService.Services;
using System.Security.Claims;

namespace PatientService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/patients")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;
        public PatientController(IPatientService service) => _service = service;

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var patient = await _service.GetByUserIdAsync(CurrentUserId());
            return patient == null ? NotFound("Patient profile not found") : Ok(patient);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("by-user/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            if (User.IsInRole("Patient") && userId != CurrentUserId()) return Forbid();

            var patient = await _service.GetByUserIdAsync(userId);
            return patient == null ? NotFound() : Ok(patient);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var patient = await _service.GetByIdAsync(id);
            if (patient == null) return NotFound();
            if (!IsAdmin() && patient.UserId != CurrentUserId()) return Forbid();
            return Ok(patient);
        }

        [Authorize(Roles = "Patient,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Patient patient)
        {
            if (!IsAdmin()) patient.UserId = CurrentUserId();
            if (await _service.GetByUserIdAsync(patient.UserId) != null)
                return BadRequest("A patient profile already exists for this user");

            var created = await _service.CreateAsync(patient);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Patient,Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Patient patient)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (!IsAdmin() && existing.UserId != CurrentUserId()) return Forbid();

            existing.Name = patient.Name;
            existing.Age = patient.Age;
            if (IsAdmin()) existing.UserId = patient.UserId;
            await _service.UpdateAsync(existing);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _service.DeleteAsync(id);
            return NoContent();
        }

        private int CurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        private bool IsAdmin() => User.IsInRole("Admin");
    }
}
