using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPreventionAPI.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")] // optional, if using API versioning
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepo _repo;
        private readonly IMapper _mapper;

        public CommentController(ICommentRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Staff,Consultant,Manager,Admin, Member")]
        public async Task<IActionResult> Create([FromBody] CreateCommentDTO dto)
        {
            Console.WriteLine($"Incoming DTO: BlogPostId={dto.BlogPostId}, ActivityId={dto.ActivityId}, ParentId={dto.ParentCommentId}");

            var comment = await _repo.AddAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = comment.Id },
                                   _mapper.Map<CommentDTO>(comment));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var comment = await _repo.GetByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(_mapper.Map<CommentDTO>(comment));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff,Consultant,Manager,Admin, Member")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentDTO dto)
        {
            var updated = await _repo.UpdateAsync(id, dto);
            return Ok(_mapper.Map<CommentDTO>(updated));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff,Consultant,Manager,Admin, Member")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager, Staff")]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
        }

        [HttpGet("blog/{postId}")]
        [Authorize(Roles = "Admin,Manager, Staff")]
        public async Task<IActionResult> GetByPost(int postId)
        {
            var comments = await _repo.GetAllByPostIdAsync(postId);
            return Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
        }

        [HttpGet("activity/{activityId}")]
        [Authorize(Roles = "Admin,Manager, Staff")]
        public async Task<IActionResult> GetByActivity(int activityId)
        {
            var comments = await _repo.GetAllByActivityIdAsync(activityId);
            return Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
        }

        [HttpGet("{parentCommentId}/replies")]
        [Authorize(Roles = "Admin,Manager, Staff")]
        public async Task<IActionResult> GetReplies(int parentCommentId)
        {
            var replies = await _repo.GetRepliesAsync(parentCommentId);
            return Ok(_mapper.Map<IEnumerable<CommentDTO>>(replies));
        }

    }
}
