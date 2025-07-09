using System.Security.Claims;
using AutoMapper;
using DrugPreventionAPI.Data;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ActivityParticipationsController : ControllerBase
    {
        private readonly IActivityParticipationRepository _repo;
        private readonly IMapper _mapper;

        public ActivityParticipationsController(IActivityParticipationRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("get-all-activity-participation")]
        [Authorize(Roles = "Admin, Manager, Staff")]
        public async Task<ActionResult<IEnumerable<ActivityParticipationDTO>>> GetAll()
        {
            var participations = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ActivityParticipationDTO>>(participations));
        }

        [HttpGet("get-by-activity/{activityId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ActivityParticipationDTO>>> GetByActivity(int activityId)
        {
            var participations = await _repo.GetByActivityIdAsync(activityId);
            return Ok(_mapper.Map<List<ActivityParticipationDTO>>(participations));
        }

        [HttpPost("register-activity")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<ActivityParticipationDTO>> Register([FromBody] int activityId)
        {
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");// from claims

            var exists = await _repo.GetByMemberAndActivityAsync(memberId, activityId); 
                if (exists != null)
                    return BadRequest("Already registered.");

            var participation = new ActivityParticipation
            {
                ActivityId = activityId,
                MemberId = memberId,
                RegistrationDate = DateTime.UtcNow,
                Status = "Registered"
            };

            var created = await _repo.RegisterAsync(participation);
            return Ok(_mapper.Map<ActivityParticipationDTO>(created));
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var success = await _repo.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpPut("update-status/{id}")]
        [Authorize(Roles = "Admin,Manager, Staff")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var updated = await _repo.UpdateStatusAsync(id, status);
            if (updated == null) return NotFound();
            return Ok(_mapper.Map<ActivityParticipationDTO>(updated));
        }

        [HttpPut("cancel/{activityId}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Cancel(int activityId)
        {
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _repo.CancelParticipationAsync(memberId, activityId);
            if (result == null)
                return NotFound("You are not registered for this activity or already cancelled.");

            return Ok(new { message = "Participation cancelled", result.Status });
        }
    }
}
