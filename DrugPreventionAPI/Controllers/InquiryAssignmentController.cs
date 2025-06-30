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
    public class InquiryAssignmentController : ControllerBase
    {
        private readonly IInquiryAssignmentRepository _repo;
        private readonly IMapper _mapper;

        public InquiryAssignmentController(
            IInquiryAssignmentRepository repo,
            IMapper mapper
        ) => (_repo, _mapper) = (repo, mapper);

        [HttpGet("get-inquiry-assignment/{inquiryId}")]
        [Authorize(Roles = "Staff,Consultant")]
        public async Task<IActionResult> GetByInquiry(int inquiryId)
        {
            var list = await _repo.GetByInquiryIdAsync(inquiryId);
            return Ok(_mapper.Map<IEnumerable<InquiryAssignmentDTO>>(list));
        }

        [HttpGet("get-assignment/{id}")]
        [Authorize(Roles = "Staff,Consultant")]
        public async Task<IActionResult> Get(int id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a == null) return NotFound();
            return Ok(_mapper.Map<InquiryAssignmentDTO>(a));
        }

        [HttpPost("create-inquiry-assignment")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create(CreateInquiryAssignment dto)
        {
            var entity = _mapper.Map<InquiryAssignment>(dto);
            var created = await _repo.AddAsync(entity);
            return CreatedAtAction(nameof(Get),
                new { id = created.Id },
                _mapper.Map<InquiryAssignmentDTO>(created));
        }

        [HttpPut("update-inquiry-assignment{id}")]
        [Authorize(Roles = "Consultant, Staff")]
        public async Task<IActionResult> Update(int id, CreateInquiryAssignment dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);
            return Ok(_mapper.Map<InquiryAssignmentDTO>(updated));
        }
    }
}
