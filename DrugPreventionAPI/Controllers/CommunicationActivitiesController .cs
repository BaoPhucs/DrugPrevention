using System.Security.Claims;
using AutoMapper;
using DrugPreventionAPI.Data;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using DrugPreventionAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunicationActivitiesController : ControllerBase

    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ICommunicationActivityRepository _repo;
        public CommunicationActivitiesController(DataContext context, IMapper mapper, ICommunicationActivityRepository repo)
        {
            _context = context;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("Get-All-Activities")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CommunicationActivityDTO>>> GetAll()
        {
            var activities = await _context.CommunicationActivities.ToListAsync();
            return Ok(activities);
        }

        [HttpGet("GetById/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CommunicationActivityDTO>> Get(int id)
        {
            var activity = await _context.CommunicationActivities.FindAsync(id);
            if (activity == null) return NotFound();
            return Ok(activity);
        }

        [HttpPost("Create-Activity")]
        [Authorize(Roles = "Staff, Manager, Consultant")]
        public async Task<ActionResult<CommunicationActivityDTO>> Create(CreateCommunicationActivityDTO dto)
        {
            var entity = _mapper.Map<CommunicationActivity>(dto);
            entity.Status = "Pending"; // default

            // Gán CreatedById từ user hiện tại
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claim, out var userId))
            {
                entity.CreatedById = userId;
            }

            _context.CommunicationActivities.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<CommunicationActivityDTO>(entity));
        }

        [HttpPut("Update-activity/{id}")]
        [Authorize(Roles = "Consultant, Staff, Manager")]
        public async Task<ActionResult<CommunicationActivityDTO>> Update(int id, CreateCommunicationActivityDTO dto)
        {
            var existing = await _context.CommunicationActivities.FindAsync(id);
            if (existing == null) return NotFound();

            // Lấy userId từ claim một cách an toàn
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out var userId))
            {
                return Forbid("User ID missing or invalid.");
            }

            // Kiểm tra quyền sở hữu
            if (existing.CreatedById.HasValue && existing.CreatedById != userId)
            {
                return Forbid("You do not have permission to update this activity.");
            }

            // Cập nhật thủ công các thuộc tính, chỉ khi giá trị hợp lệ và không phải mặc định
            if (dto.Title != null && !string.IsNullOrWhiteSpace(dto.Title) && dto.Title != "string")
            {
                existing.Title = dto.Title;
            }

            if (dto.Description != null && !string.IsNullOrWhiteSpace(dto.Description) && dto.Description != "string")
            {
                existing.Description = dto.Description;
            }

            if (dto.EventDate != default && dto.EventDate > DateTime.UtcNow)
            {
                existing.EventDate = dto.EventDate;
            }

            if (dto.Location != null && !string.IsNullOrWhiteSpace(dto.Location) && dto.Location != "string")
            {
                existing.Location = dto.Location;
            }

            if (dto.Capacity.HasValue && dto.Capacity > 0) // Loại bỏ giá trị 0 mặc định
            {
                existing.Capacity = dto.Capacity;
            }

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<CommunicationActivityDTO>(existing));
        }

        [HttpDelete("Delete-Activity/{id}")]
        [Authorize(Roles = "Manager, Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repo.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("Submit-For-Approval/{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<CommunicationActivityDTO>> SubmitForApproval(int id)
        {
            var activity = await _repo.SubmitForApprovalAsync(id);
            if (activity == null) return BadRequest("Cannot submit for approval. Activity may not be in Pending status.");
            return Ok(_mapper.Map<CommunicationActivityDTO>(activity));
        }

        [HttpPost("Approve/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<CommunicationActivityDTO>> Approve(int id)
        {
            var activity = await _repo.ApproveAsync(id);
            if (activity == null) return BadRequest("Cannot approve. Activity may not be in Submitted status.");
            return Ok(_mapper.Map<CommunicationActivityDTO>(activity));
        }

        [HttpPost("Reject/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<CommunicationActivityDTO>> Reject(int id, [FromQuery] string? reviewComments)
        {
            var activity = await _repo.RejectAsync(id, reviewComments);
            if (activity == null) return BadRequest("Cannot reject. Activity may not be in Submitted status.");
            return Ok(_mapper.Map<CommunicationActivityDTO>(activity));
        }

        [HttpPost("Publish/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<CommunicationActivityDTO>> Publish(int id)
        {
            var activity = await _repo.PublishAsync(id);
            if (activity == null) return BadRequest("Cannot publish. Activity may not be in Approved status.");
            return Ok(_mapper.Map<CommunicationActivityDTO>(activity));
        }
    }
}
