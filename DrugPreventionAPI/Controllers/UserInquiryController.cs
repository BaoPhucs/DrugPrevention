using System.Linq;
using System.Security.Claims;
using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserInquiryController : ControllerBase
    {
        private readonly IUserInquiryRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public UserInquiryController(
            IUserInquiryRepository repo,
            IMapper mapper,
            IHttpContextAccessor http
        ) => (_repo, _mapper, _http) = (repo, mapper, http);


        // debug endpoint to see exactly what claims you have
        [HttpGet("claims")]
        public IActionResult ListClaims()
        {
            var claims = User.Claims
                             .Select(c => new { c.Type, c.Value })
                             .ToList();
            return Ok(claims);
        }


        // Guests & Members can create
        [HttpPost("create-inquiry")]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateUserInquiryDTO dto)
        {
            var iq = _mapper.Map<UserInquiry>(dto);

            // stamp CreatedById from the NameIdentifier claim, if present
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claim, out var userId))
                iq.CreatedById = userId;

            var created = await _repo.AddAsync(iq);
            return CreatedAtAction(nameof(Get), new { id = created.Id },
                                   _mapper.Map<UserInquiryDTO>(created));
        }


        // Members list their own
        [HttpGet("myInquiries")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyInquiries()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
                return Unauthorized("User ID missing or invalid.");

            var list = await _repo.GetByUserAsync(userId);
            return Ok(_mapper.Map<IEnumerable<UserInquiryDTO>>(list));
        }


        // Staff list all
        [HttpGet("get-inquiry-by-staff")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<UserInquiryDTO>>(list));
        }


        // Get by ID (with Member/Consultant/Manager/Admin guards)
        [HttpGet("get-inquiry/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var iq = await _repo.GetByIdAsync(id);
            if (iq == null) return NotFound();

            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
                return Unauthorized("User ID missing or invalid.");

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Member" && iq.CreatedById != userId)
                return Forbid();

            if ((role == "Consultant" || role == "Manager" || role == "Admin")
                && !iq.InquiryAssignments.Any(a => a.AssignedToId == userId))
                return Forbid();

            return Ok(_mapper.Map<UserInquiryDTO>(iq));
        }


        // Update – only Members may update their own
        [HttpPut("update-inquiry/{id:int}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Update(int id, UpdateUserInquiryDTO dto)
        {
            var iq = await _repo.GetByIdAsync(id);
            if (iq == null) return NotFound();

            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId) || iq.CreatedById != userId)
                return Forbid();

            _mapper.Map(dto, iq);
            var updated = await _repo.UpdateAsync(iq);
            return Ok(_mapper.Map<UserInquiryDTO>(updated));
        }


        // Delete – Consultant/Manager/Admin, with “assigned to” check for Consultants only
        [HttpDelete("delete-inquiry/{id:int}")]
        [Authorize(Roles = "Consultant,Manager,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var iq = await _repo.GetByIdAsync(id);
            if (iq == null) return NotFound();

            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
                return Unauthorized("User ID missing or invalid.");

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // Consultants may only delete if assigned
            if (role == "Consultant"
                && !iq.InquiryAssignments.Any(a => a.AssignedToId == userId))
                return Forbid();

            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
