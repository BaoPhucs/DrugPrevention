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
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostRepo _repo;
        private readonly IMapper _mapper;
        public BlogPostController(IBlogPostRepo repo, IMapper mapper) => (_repo, _mapper) = (repo, mapper);

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(_mapper.Map<IEnumerable<BlogPostDTO>>(await _repo.GetAllAsync()));

        [HttpGet("{get-BlogPostId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var bp = await _repo.GetByIdAsync(id);
            if (bp == null) return NotFound();
            return Ok(_mapper.Map<BlogPostDTO>(bp));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateBlogPostDTO dto)
        {
            var post = _mapper.Map<BlogPost>(dto);
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claim, out var uid)) post.CreatedById = uid;
            var created = await _repo.AddAsync(post, dto.TagIds);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, _mapper.Map<BlogPostDTO>(created));
        }

        [HttpPut("{update-tagId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateBlogPostDTO dto)
        {
            var post = await _repo.GetByIdAsync(id);
            if (post == null) return NotFound();
            _mapper.Map(dto, post);
            var updated = await _repo.UpdateAsync(post, dto.TagIds);
            return Ok(_mapper.Map<BlogPostDTO>(updated));
        }

        [HttpDelete("{BlogPostId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }

    }
}