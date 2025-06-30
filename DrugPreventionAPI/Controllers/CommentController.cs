using System.Security.Claims;
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

        [HttpPost("createNewComment-allowAllExceptGuest")]
        [Authorize(Roles = "Staff,Consultant,Manager,Admin, Member")]
        public async Task<IActionResult> Create([FromBody] CreateCommentDTO dto)
        {

            if (dto is null || dto.Content is null)
                return BadRequest("Missing content.");

            Console.WriteLine($"Incoming DTO: BlogPostId={dto.BlogPostId}, ActivityId={dto.ActivityId}");

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

        [HttpPut("update-GuestCannot/{id}")]
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

        [HttpGet("GetAll-OnlyAdminManagerStaff")]
        [Authorize(Roles = "Admin,Manager, Staff")]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
        }

        [HttpGet("GetCommentByBlogPost/{postId}")]
        [Authorize(Roles = "Admin,Manager, Staff")]
        public async Task<IActionResult> GetByPost(int postId)
        {
            var comments = await _repo.GetAllByPostIdAsync(postId);
            return Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
        }

        [HttpGet("GetAllCommentsByActivity/{activityId}")]
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

        [HttpPost("reply-allowAllExceptGuest")]
        [Authorize(Roles = "Staff,Consultant,Manager,Admin,Member")]
        public async Task<IActionResult> CreateReply([FromBody] CreateReplyDTO dto)
        {
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"); // extension method
            var reply = await _repo.AddReplyAsync(dto, memberId);
            return CreatedAtAction(nameof(GetById),
                new { id = reply.Id },
                _mapper.Map<CommentDTO>(reply));
        }

    }
}
