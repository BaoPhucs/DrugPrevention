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
        private readonly IEmailService _emailService;
        private readonly INotificationRepository _noteRepo;
        private readonly IConsultantScheduleRepository _consultantScheduleRepository;
        public AppointmentRequestController(IAppointmentRequestRepository appointmentRequestRepository, IMapper mapper, IEmailService emailService, INotificationRepository noteRepo, IConsultantScheduleRepository consultantScheduleRepository)
        {
            _appointmentRequestRepository = appointmentRequestRepository;
            _mapper = mapper;
            _emailService = emailService;
            _noteRepo = noteRepo;
            _consultantScheduleRepository = consultantScheduleRepository;
        }

        private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequestDTO dto)
        {
            // 1) Lấy slot từ repository
            var slot = await _consultantScheduleRepository.GetScheduleById(dto.ScheduleId);
            if (slot == null || slot.ScheduleDate == null || slot.StartTime == null)
                return BadRequest("Khung giờ không hợp lệ.");

            // 2) Ghép DateOnly + TimeOnly => DateTime
            var appointmentDateTime = slot.ScheduleDate.Value
                                          .ToDateTime(slot.StartTime.Value);

            // 3) Kiểm tra đặt trước 24h
            if (appointmentDateTime <= DateTime.UtcNow.AddHours(24))
                return BadRequest("Bạn phải đặt lịch ít nhất 24 giờ trước khi cuộc hẹn.");

            // 4) Tiếp tục tạo request
            var req = new AppointmentRequest
            {
                MemberId = CurrentUserId,
                ConsultantId = dto.ConsultantId,
                ScheduleId = dto.ScheduleId,
                Status = "Pending"
            };
            var created = await _appointmentRequestRepository.CreateAsync(req);
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

        // 11) Consultant đánh dấu No Show
        [HttpPut("{id:int}/noshow")]
        [Authorize(Roles = "Consultant,Manager")]
        public async Task<IActionResult> MarkNoShow(int id, [FromBody] NoShowDTO dto)
        {
            var ok = await _appointmentRequestRepository.MarkNoShowAsync(id, dto.Reason);
            if (!ok) return NotFound();

            // Lấy lại request để có email, userId
            var req = await _appointmentRequestRepository.GetByIdAsync(id);
            var userEmail = req.Member.Email;
            var userId = req.MemberId;

            // Tạo Notification record
            var note = new Notification
            {
                UserId = userId,
                Type = "NoShowWarning",
                Title = "Cảnh báo: Không đến cuộc hẹn",
                Message = $"Bạn đã không đến cuộc hẹn {req.Schedule.ScheduleDate:dd/MM/yyyy}. " +
                           $"Lần này được đánh dấu No-Show.",
                SendDate = DateTime.UtcNow
            };
            await _noteRepo.CreateAsync(note);

            // Gửi email
            var html = $@"
            <p>Xin chào {req.Member.Name},</p>
            <p>Bạn đã không tham dự cuộc hẹn tư vấn vào ngày " +
                $"<strong>{req.Schedule.ScheduleDate:dd/MM/yyyy}</strong>.</p>" +
                "<p>Vui lòng chú ý thời gian hoặc liên hệ lại để đặt lịch mới.</p>";
            await _emailService.SendEmailAsync(
                userEmail,
                "Cảnh báo: Bạn không đến cuộc hẹn tư vấn",
                html);

            return NoContent();
        }
    }
}
