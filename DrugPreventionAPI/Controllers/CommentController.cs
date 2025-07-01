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

        [HttpPost("create-comment")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCommentDTO dto)
        {
            if (dto == null || dto.Content == null)
                return BadRequest("Missing content.");

            Console.WriteLine($"Incoming DTO: BlogPostId={dto.BlogPostId}, ActivityId={dto.ActivityId}");

            var comment = _mapper.Map<Comment>(dto);
            comment.MemberId = CurrentUserId; // Gán ID người dùng hiện tại
            comment.CreatedDate = DateTime.UtcNow;

            if (comment.BlogPostId == null && comment.ActivityId == null)
                return BadRequest("A root comment must target either a BlogPost or an Activity.");

            if (comment.BlogPostId != null && comment.ActivityId != null)
                return BadRequest("A comment must not target both a BlogPost and an Activity.");

            var created = await _repo.AddAsync(comment);
            return CreatedAtAction(nameof(GetById),
                                   new { id = created.Id },
                                   _mapper.Map<CommentDTO>(created));
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
        [Authorize]
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
                MemberId = CurrentUserId, // Gán ID người dùng hiện tại
                CreatedDate = DateTime.UtcNow
            };

            var created = await _repo.AddAsync(reply);
            return CreatedAtAction(nameof(GetById),
                new { id = created.Id },
                _mapper.Map<CommentDTO>(created));
        }

    }
}
