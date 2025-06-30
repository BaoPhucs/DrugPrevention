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
        private readonly IAppointmentRequestRepository _appointmentRequestRepository;
        private readonly IMapper _mapper;
        public ConsultationNoteController(IConsultationNoteRepository consultationNoteRepository, IMapper mapper, IAppointmentRequestRepository appointmentRequestRepository)
        {
            _consultationNoteRepository = consultationNoteRepository;
            _mapper = mapper;
            _appointmentRequestRepository = appointmentRequestRepository;
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
            // 1) Lấy appointment
            var appt = await _appointmentRequestRepository.GetByIdAsync(appointmentId);
            if (appt == null) return NotFound($"Appointment {appointmentId} không tồn tại.");

            // 2) Khởi tạo note
            var note = new ConsultationNote
            {
                AppointmentId = appointmentId,
                ConsultantId = CurrentUserId,
                MemberId = appt.MemberId ?? 0,
                Notes = dto.Notes,
                CreatedDate = DateTime.UtcNow
            };

            // 3) Thêm note
            var created = await _consultationNoteRepository.AddAsync(note);

            // 4) Ánh xạ sang DTO và trả về 201
            var responseDto = _mapper.Map<ConsultationNoteDTO>(created);
            return CreatedAtAction(nameof(GetAll), new { appointmentId }, responseDto);
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
