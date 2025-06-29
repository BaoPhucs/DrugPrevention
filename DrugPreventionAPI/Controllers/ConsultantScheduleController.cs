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
    public class ConsultantScheduleController : ControllerBase
    {
        private readonly IConsultantScheduleRepository _consultantScheduleRepository;
        private readonly IMapper _mapper;
        public ConsultantScheduleController(IConsultantScheduleRepository consultantScheduleRepository, IMapper mapper)
        {
            _consultantScheduleRepository = consultantScheduleRepository;
            _mapper = mapper;
        }
        private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [HttpGet("{consultantId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByConsultant(int consultantId)
        {
            var list = await _consultantScheduleRepository.GetAvailableByConsultantAsync(consultantId);
            return Ok(_mapper.Map<IEnumerable<ConsultantScheduleDTO>>(list));
        }

        [HttpGet("get-all-consultant")]
        [Authorize]
        public async Task<IActionResult> GetConsutant()
        {
            var list = await _consultantScheduleRepository.GetConsultant();
            return Ok(_mapper.Map<IEnumerable<UserDTO>>(list));
        }

        [HttpGet("get-schedule/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetById(int scheduleId)
        {
            var schedule = await _consultantScheduleRepository.GetScheduleById(scheduleId);
            if (schedule == null) return NotFound();
            return Ok(_mapper.Map<ConsultantScheduleDTO>(schedule));
        }

        [HttpGet("availability/{isAvailable:bool}")]
        //[Authorize(Roles = "Consultant")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByAvailability(bool isAvailable)
        {
            var list = await _consultantScheduleRepository.GetByIsAvailabilityAsync(isAvailable);
            return Ok(_mapper.Map<IEnumerable<ConsultantScheduleDTO>>(list));
        }

        [HttpPost("add-schedule")]
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> Create([FromBody] CreateConsultantScheduleDTO dto)
        {
            var slot = new ConsultantSchedule
            {
                ConsultantId = CurrentUserId,
                ScheduleDate = dto.ScheduleDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsAvailable = true
            };
            var created = await _consultantScheduleRepository.AddAsync(slot);
            return CreatedAtAction(nameof(GetByConsultant),
                new { consultantId = CurrentUserId },
                _mapper.Map<ConsultantScheduleDTO>(created));
        }

        [HttpPut("update-schedule/{id:int}")]
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> Update(int id, [FromBody] ConsultantScheduleDTO dto)
        {
            if (dto.Id != id) return BadRequest();
            var slot = new ConsultantSchedule
            {
                Id = id,
                ConsultantId = CurrentUserId,
                ScheduleDate = dto.ScheduleDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsAvailable = true
            };
            return await _consultantScheduleRepository.UpdateAsync(slot)
                 ? NoContent()
                 : NotFound();
        }

        [HttpDelete("delete-schedule/{id:int}")]
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> Delete(int id)
            => await _consultantScheduleRepository.DeleteAsync(id) ? NoContent() : NotFound();

    }
}
