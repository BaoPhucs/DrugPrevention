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

        [HttpGet("get-all-blogpost")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(_mapper.Map<IEnumerable<BlogPostDTO>>(await _repo.GetAllAsync()));

        [HttpGet("get-blogpost-by-id/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var bp = await _repo.GetByIdAsync(id);
            if (bp == null) return NotFound();
            return Ok(_mapper.Map<BlogPostDTO>(bp));
        }

        [HttpGet("get-blogpost-by-tag/{tagId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByTag(int tagId)
        {
            var bp = await _repo.GetByTagId(tagId);
            if (bp == null) return NotFound();
            return Ok(_mapper.Map<IEnumerable<BlogPostDTO>>(bp));
        }

        [HttpPost("create-blogpost")]
        [Authorize(Roles = "Manager, Staff, Consultant")]
        public async Task<IActionResult> Create(CreateBlogPostDTO dto)
        {
            var post = _mapper.Map<BlogPost>(dto);
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claim, out var uid)) post.CreatedById = uid;
            var created = await _repo.AddAsync(post, dto.TagIds);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, _mapper.Map<BlogPostDTO>(created));
        }

        [HttpPut("update-blogpost/{BlogPostId}")]
        [Authorize(Roles = "Manager, Staff, Consultant")]
        public async Task<IActionResult> Update(int BlogPostId, UpdateBlogPostDTO dto)
        {

            var updated = await _repo.UpdateAsync(BlogPostId, dto);
            return Ok(_mapper.Map<BlogPostDTO>(updated));
        }

        [HttpDelete("delete-blogpost/{BlogPostId}")]
        [Authorize(Roles = "Manager, Staff, Consultant")]
        public async Task<IActionResult> Delete(int BlogPostId)
        {
            await _repo.DeleteAsync(BlogPostId);
            return NoContent();
        }

        [HttpPost("submit-for-approval/{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> SubmitForApproval(int id)
        {
            var post = await _repo.SubmitForApprovalAsync(id);
            if (post == null) return BadRequest("Cannot submit for approval. Blog post may not be in Pending status.");
            return Ok(_mapper.Map<BlogPostDTO>(post));
        }

        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            var post = await _repo.ApproveAsync(id);
            if (post == null) return BadRequest("Cannot approve. Blog post may not be in Submitted status.");
            return Ok(_mapper.Map<BlogPostDTO>(post));
        }

        [HttpPost("reject/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Reject(int id, [FromQuery] string? reviewComments)
        {
            var post = await _repo.RejectAsync(id, reviewComments);
            if (post == null) return BadRequest("Cannot reject. Blog post may not be in Submitted status.");
            return Ok(_mapper.Map<BlogPostDTO>(post));
        }

        [HttpPost("publish/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Publish(int id)
        {
            var post = await _repo.PublishAsync(id);
            if (post == null) return BadRequest("Cannot publish. Blog post may not be in Approved status.");
            return Ok(_mapper.Map<BlogPostDTO>(post));
        }
    }
}