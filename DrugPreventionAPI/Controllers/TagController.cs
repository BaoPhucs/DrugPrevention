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
    public class TagController : ControllerBase
    {
        private readonly ITagRepo _repo;
        private readonly IMapper _mapper;
        public TagController(ITagRepo repo, IMapper mapper) => (_repo, _mapper) = (repo, mapper);

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(_mapper.Map<IEnumerable<TagDTO>>(await _repo.GetAllAsync()));

        [HttpGet("{tagid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int tagid)
        {
            var t = await _repo.GetByIdAsync(tagid);
            if (t == null) return NotFound();
            return Ok(_mapper.Map<TagDTO>(t));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Create(CreateTagDTO dto)
        {
            var tag = _mapper.Map<Tag>(dto);
            var created = await _repo.AddAsync(tag);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, _mapper.Map<TagDTO>(created));
        }

        [HttpPut("{tagid}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Update(int tagid, CreateTagDTO dto)
        {
            var tag = await _repo.GetByIdAsync(tagid);
            if (tag == null) return NotFound();
            _mapper.Map(dto, tag);
            var updated = await _repo.UpdateAsync(tag);
            return Ok(_mapper.Map<TagDTO>(updated));
        }

        [HttpDelete("{tagid}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Delete(int tagid)
        {
            await _repo.DeleteAsync(tagid);
            return NoContent();
        }
    }
}
