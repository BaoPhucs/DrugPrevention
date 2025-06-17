using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InquiryCommentController : ControllerBase
    {
        private readonly IInquiryCommentRepository _repo;
        private readonly IMapper _mapper;

        public InquiryCommentController(
            IInquiryCommentRepository repo,
            IMapper mapper
        ) => (_repo, _mapper) = (repo, mapper);

        [HttpGet("inquiry/{inquiryId:int}")]
        public async Task<IActionResult> GetByInquiry(int inquiryId)
        {
            var list = await _repo.GetByInquiryAsync(inquiryId);
            return Ok(_mapper.Map<IEnumerable<InquiryCommentDTO>>(list));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var comment = await _repo.GetByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(_mapper.Map<InquiryCommentDTO>(comment));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInquiryCommentDTO dto)
        {
            var entity = _mapper.Map<InquiryComment>(dto);
            var created = await _repo.AddAsync(entity);
            return CreatedAtAction(nameof(Get),
            new { id = created.Id },
                _mapper.Map<InquiryCommentDTO>(created));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CreateInquiryCommentDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);
            return Ok(_mapper.Map<InquiryCommentDTO>(updated));
        }
    }

}
