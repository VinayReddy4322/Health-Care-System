using DoctorService.Models;
using DoctorService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _service;
        public DoctorController(IDoctorService service) => _service = service;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var doctor = await _service.GetByUserIdAsync(CurrentUserId());
            return doctor == null ? NotFound("Doctor profile not found") : Ok(doctor);
        }

        [AllowAnonymous]
        [HttpGet("by-user/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var doctor = await _service.GetByUserIdAsync(userId);
            return doctor == null ? NotFound() : Ok(doctor);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var doctor = await _service.GetByIdAsync(id);
            return doctor == null ? NotFound() : Ok(doctor);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Doctor doctor)
        {
            if (!User.IsInRole("Admin")) doctor.UserId = CurrentUserId();
            if (await _service.GetByUserIdAsync(doctor.UserId) != null)
                return BadRequest("A doctor profile already exists for this user");

            var created = await _service.CreateAsync(doctor);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Doctor doctor)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (!User.IsInRole("Admin") && existing.UserId != CurrentUserId()) return Forbid();

            existing.Name = doctor.Name;
            existing.Specialization = doctor.Specialization;
            if (User.IsInRole("Admin")) existing.UserId = doctor.UserId;
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
    }
}
