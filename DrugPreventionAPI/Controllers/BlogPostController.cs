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

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var bp = await _repo.GetByIdAsync(id);
            if (bp == null) return NotFound();
            return Ok(_mapper.Map<BlogPostDTO>(bp));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Create(CreateBlogPostDTO dto)
        {
            var post = _mapper.Map<BlogPost>(dto);
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claim, out var uid)) post.CreatedById = uid;
            var created = await _repo.AddAsync(post, dto.TagIds);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, _mapper.Map<BlogPostDTO>(created));
        }

        [HttpPut("{Id}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Update(int Id, UpdateBlogPostDTO dto)
        {
          
            var updated = await _repo.UpdateAsync(Id, dto);
            return Ok(_mapper.Map<BlogPostDTO>(updated));
        }

        [HttpDelete("{BlogPostId}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Delete(int BlogPostId)
        {
            await _repo.DeleteAsync(BlogPostId);
            return NoContent();
        }

    }
}