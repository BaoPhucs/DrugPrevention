using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentRequestController : ControllerBase
    {
        private readonly IAppointmentRequestRepository _appointmentRequestRepository;
        private readonly IMapper _mapper;
        public AppointmentRequestController(IAppointmentRequestRepository appointmentRequestRepository, IMapper mapper)
        {
            _appointmentRequestRepository = appointmentRequestRepository;
            _mapper = mapper;
        }

        private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequestDTO dto)
        {
            var req = new AppointmentRequest
            {
                MemberId = CurrentUserId,
                ConsultantId = dto.ConsultantId,
                ScheduleId = dto.ScheduleId,
                Status = "Pending"
            };
            var created = await _appointmentRequestRepository.CreateAsync(req);
            // Map sang DTO để không include navigation
            var resultDto = _mapper.Map<AppointmentRequestDTO>(created);
            return CreatedAtAction(nameof(GetById), new { id = resultDto.Id }, resultDto);
        }

        [HttpGet("users/{memberId:int}")]
        [Authorize]
        public async Task<IActionResult> GetByUser(int memberId)
        {
            var cur = CurrentUserId;
            if (cur != memberId && !User.IsInRole("Consultant") && !User.IsInRole("Manager"))
                return Forbid();
            var list = await _appointmentRequestRepository.GetByMemberAsync(memberId);
            return Ok(list);
        }

        [HttpGet("consultants/{consultantId:int}")]
        [Authorize(Roles = "Consultant,Staff,Manager")]
        public async Task<IActionResult> GetByConsultant(int consultantId)
        {
            return Ok(await _appointmentRequestRepository.GetByConsultantAsync(consultantId));
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _appointmentRequestRepository.GetByIdAsync(id);
            return r == null ? NotFound() : Ok(r);
        }

        [HttpGet("get-all-appointment")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _appointmentRequestRepository.GetAllAsync();
            return Ok(list);
        }

        [HttpPut("{id:int}/update-status")]
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusDTO dto)
        {
            return await _appointmentRequestRepository.UpdateStatusAsync(id, dto.Status, dto.CancelReason)
             ? NoContent()
             : NotFound();
        }

        // 10) Member huỷ request (xóa hẳn) trước khi consultant confirm
        [HttpDelete("{requestId:int}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Cancel(int requestId)
        {
            // chỉ cho phép chính chủ huỷ
            var req = await _appointmentRequestRepository.GetByIdAsync(requestId);
            if (req == null) return NotFound($"Request {requestId} không tồn tại.");
            if (req.MemberId != CurrentUserId) return Forbid();

            var success = await _appointmentRequestRepository.DeleteAsync(requestId);
            if (!success) return StatusCode(500, "Không thể huỷ request này.");
            return NoContent(); // 204
        }
    }
}
