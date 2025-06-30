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
            return Ok(_mapper.Map<List<CommunicationActivityDTO>>(activities));
        }

        [HttpGet("GetById/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CommunicationActivityDTO>> Get(int id)
        {
            var activity = await _context.CommunicationActivities.FindAsync(id);
            if (activity == null) return NotFound();
            return Ok(_mapper.Map<CommunicationActivityDTO>(activity));
        }

        [HttpPost("Create-Activity")]
        [Authorize(Roles = "Admin,Staff, Manager")]
        public async Task<ActionResult<CommunicationActivityDTO>> Create(CreateCommunicationActivityDTO dto)
        {
            var entity = _mapper.Map<CommunicationActivity>(dto);
            entity.Status = "Pending"; // default
            _context.CommunicationActivities.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<CommunicationActivityDTO>(entity));
        }

        [HttpPut("Update-activity/{id}")]
        [Authorize(Roles = "Admin,Staff, Manager")]
        public async Task<ActionResult<CommunicationActivityDTO>> Update(int id, CreateCommunicationActivityDTO dto)
        {
            var activity = _mapper.Map<CommunicationActivity>(dto);
            var updated = await _repo.UpdateAsync(id, activity);
            if (updated == null) return NotFound();

            return Ok(_mapper.Map<CommunicationActivityDTO>(updated));
        }

        [HttpDelete("Delete-Activity/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repo.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

    }
}
