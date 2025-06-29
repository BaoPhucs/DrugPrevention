using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using DrugPreventionAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultationNoteController : ControllerBase
    {
        private readonly IConsultationNoteRepository _consultationNoteRepository;
        private readonly IMapper _mapper;
        public ConsultationNoteController(IConsultationNoteRepository consultationNoteRepository, IMapper mapper)
        {
            _consultationNoteRepository = consultationNoteRepository;
            _mapper = mapper;
        }
        private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [HttpGet("get-note")]
        [Authorize]
        public async Task<IActionResult> GetAll(int appointmentId)
        {
            var notes = await _consultationNoteRepository.GetByAppointmentAsync(appointmentId);
            return Ok(notes);
        }

        [HttpPost("add-note")]
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> Add(int appointmentId, [FromBody] CreateConsultationNoteDTO dto)
        {
            var note = new ConsultationNote
            {
                AppointmentId = appointmentId,
                ConsultantId = CurrentUserId,
                MemberId = (await _consultationNoteRepository.GetByAppointmentAsync(appointmentId))
                                    .First().MemberId, // lấy MemberId từ request
                Notes = dto.Notes
            };
            var created = await _consultationNoteRepository.AddAsync(note);
            return CreatedAtAction(nameof(GetAll),
                new { appointmentId }, created);
        }

        [HttpPut("update-note/{noteId}")]
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateConsultationNoteDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Notes))
                return BadRequest("Notes cannot be empty.");

            var ok = await _consultationNoteRepository.UpdateNoteAssync(id, dto.Notes);
            if (!ok)
                return NotFound($"ConsultationNote with ID {id} not found.");

            return NoContent();
        }

    }
}
