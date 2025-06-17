// Controllers/UserInquiryController.cs
using System.Security.Claims;
using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Authorization;
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

        //–– Create a new inquiry –– Guests and Members
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateUserInquiryDTO dto)
        {
            var iq = _mapper.Map<UserInquiry>(dto);
            var userId = _http.HttpContext.User.FindFirst("id")?.Value;
            if (userId != null) iq.CreatedById = int.Parse(userId);

            var created = await _repo.AddAsync(iq);
            return CreatedAtAction(nameof(Get), new { id = created.Id },
                                   _mapper.Map<UserInquiryDTO>(created));
        }

        //–– Members list their own inquiries
        [HttpGet("my")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyInquiries()
        {
            var userId = int.Parse(_http.HttpContext.User.FindFirst("id")!.Value);
            var list = await _repo.GetByUserAsync(userId);
            return Ok(_mapper.Map<IEnumerable<UserInquiryDTO>>(list));
        }

        //–– Staff list all inquiries
        [HttpGet]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<UserInquiryDTO>>(list));
        }

        //–– Get by ID –– either:
        //   • Staff (any),  
        //   • Member (own),  
        //   • Consultant/Manager/Admin (only if assigned)
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var iq = await _repo.GetByIdAsync(id);
            if (iq == null) return NotFound();

            var userId = int.Parse(_http.HttpContext.User.FindFirst("id")!.Value);
            var role = _http.HttpContext.User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "Member" && iq.CreatedById != userId)
                return Forbid();

            if ((role == "Consultant" || role == "Manager" || role == "Admin")
                && !iq.InquiryAssignments.Any(a => a.AssignedToId == userId))
                return Forbid();

            // Staff passes through
            return Ok(_mapper.Map<UserInquiryDTO>(iq));
        }

        //–– Update –– only Members may update *their* inquiries
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Update(int id, UpdateUserInquiryDTO dto)
        {
            var iq = await _repo.GetByIdAsync(id);
            if (iq == null) return NotFound();

            var userId = int.Parse(_http.HttpContext.User.FindFirst("id")!.Value);
            if (iq.CreatedById != userId)
                return Forbid();

            _mapper.Map(dto, iq);
            var updated = await _repo.UpdateAsync(iq);
            return Ok(_mapper.Map<UserInquiryDTO>(updated));
        }

        //–– Delete –– Staff,Consultant only if assigned; /Manager/Admin may delete any
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Consultant,Manager,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var iq = await _repo.GetByIdAsync(id);
            if (iq == null) return NotFound();

            var userId = int.Parse(_http.HttpContext.User.FindFirst("id")!.Value);
            var role = _http.HttpContext.User.FindFirst(ClaimTypes.Role)!.Value;

            if ((role == "Consultant" || role == "Staff" )
                && !iq.InquiryAssignments.Any(a => a.AssignedToId == userId))
                return Forbid();

            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
