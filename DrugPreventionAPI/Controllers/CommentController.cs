using System.Security.Claims;
using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using DrugPreventionAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPreventionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepo _repo;
        private readonly IMapper _mapper;
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        public CommentController(ICommentRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpPost("create-blogpost-comment")]
        [Authorize]
        public async Task<IActionResult> CreateBlogPostComment([FromBody] CreateBlogPostCommentDTO dto)
        {
            if (dto == null || dto.Content == null)
                return BadRequest("Missing content.");

            var comment = await _repo.AddBlogPostCommentAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById),
                                  new { id = comment.Id },
                                  _mapper.Map<CommentDTO>(comment));
        }

        [HttpPost("create-activity-comment")]
        [Authorize]
        public async Task<IActionResult> CreateActivityComment([FromBody] CreateActivityCommentDTO dto)
        {
            if (dto == null || dto.Content == null)
                return BadRequest("Missing content.");

            var comment = await _repo.AddActivityCommentAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById),
                                  new { id = comment.Id },
                                  _mapper.Map<CommentDTO>(comment));
        }

        [HttpGet("get/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var comment = await _repo.GetByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(_mapper.Map<CommentDTO>(comment));
        }

        [HttpPut("update-comment/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentDTO dto)
        {
            var updated = await _repo.UpdateAsync(id, dto);
            return Ok(_mapper.Map<CommentDTO>(updated));
        }

        [HttpDelete("delete-comment/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("get-post-comment/{postId}")]
        [Authorize]
        public async Task<IActionResult> GetByPost(int postId)
        {
            var comments = await _repo.GetAllByPostIdAsync(postId);
            return Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
        }

        [HttpGet("get-by-activity/{activityId}")]
        [Authorize]
        public async Task<IActionResult> GetByActivity(int activityId)
        {
            var comments = await _repo.GetAllByActivityIdAsync(activityId);
            return Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
        }

        [HttpGet("{parentCommentId}/replies")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReplies(int parentCommentId)
        {
            var replies = await _repo.GetRepliesAsync(parentCommentId);
            return Ok(_mapper.Map<IEnumerable<CommentDTO>>(replies));
        }

        [HttpPost("reply-comment")]
        [Authorize]
        public async Task<IActionResult> CreateReply([FromBody] CreateReplyDTO dto)
        {
            var reply = new Comment
            {
                ParentCommentId = dto.ParentCommentId,
                Content = dto.Content,
                MemberId = CurrentUserId,
                CreatedDate = DateTime.UtcNow
            };

            var created = await _repo.AddReplyAsync(reply);
            return CreatedAtAction(nameof(GetById),
                new { id = created.Id },
                _mapper.Map<CommentDTO>(created));
        }

    }
}
