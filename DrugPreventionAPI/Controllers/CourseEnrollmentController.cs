using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseEnrollmentController : ControllerBase
    {
        private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
        private readonly IMapper _mapper;
        public CourseEnrollmentController(ICourseEnrollmentRepository courseEnrollmentRepository, IMapper mapper)
        {
            _courseEnrollmentRepository = courseEnrollmentRepository;
            _mapper = mapper;
        }

        [HttpPost("courses/{courseId:int}/enroll")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Enroll(int courseId, [FromBody] int? memberId = null)
        {
            // nếu không truyền body, lấy từ token
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = memberId ?? (idClaim != null ? int.Parse(idClaim) : 0);
            if (userId <= 0) return Unauthorized();

            var ok = await _courseEnrollmentRepository.EnrollAsync(courseId, userId);
            if (!ok) return BadRequest("Unable to enroll (maybe already enrolled)");
            return NoContent();
        }

        [HttpDelete("courses/{courseId:int}/unenroll")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Cancel(int courseId)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId) || userId <= 0)
                return Unauthorized();

            var ok = await _courseEnrollmentRepository.CancelEnrollmentAsync(courseId, userId);
            if (!ok) return NotFound("Enrollment not found");
            return NoContent();
        }


        [HttpGet("courses/{courseId:int}/enrollment-status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEnrollmentStatus(int courseId, [FromQuery] int? memberId = null)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = memberId ?? (idClaim != null ? int.Parse(idClaim) : 0);
            if (userId <= 0) return Unauthorized();
            var enrollment = await _courseEnrollmentRepository.GetEnrollmentStatusAsync(courseId, userId);
            var enrollmentDto = _mapper.Map<CourseEnrollmentDTO>(enrollment);
            if (enrollmentDto == null) return NotFound("Enrollment not found");
            return Ok(enrollmentDto);
        }

        [HttpGet("users/{memberId:int}/enrollments")]
        [Authorize]
        public async Task<IActionResult> GetUserEnrollments(int memberId)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var curId = idClaim != null ? int.Parse(idClaim) : 0;
            if(curId != memberId && (!User.IsInRole("Admin") && !User.IsInRole("Consultant")))
            {
                return Forbid();
            }
            
            var enrollments = await _courseEnrollmentRepository.GetEnrollmentsByUserAsync(memberId);
            var dtos = _mapper.Map<IEnumerable<CourseEnrollmentDTO>>(enrollments);
            return Ok(dtos);
        }

        [HttpGet("courses/{courseId:int}/enrollments")]
        [Authorize(Roles = "Admin, Manager, Staff")]
        public async Task<IActionResult> GetEnrollmentsByCourse(int courseId)
        {
            var list = await _courseEnrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
            if (list == null || !list.Any())
                return NotFound($"No enrollments found for course {courseId}");

            
            var dtos = _mapper.Map<IEnumerable<CourseEnrollmentDTO>>(list);
            return Ok(dtos);
        }

        [HttpGet("courses/{courseId:int}/enrollment-count")]
        [Authorize(Roles = "Admin, Manager, Staff")]
        public async Task<IActionResult> GetEnrollmentCount(int courseId)
        {
            var cnt = await _courseEnrollmentRepository.GetEnrollmentCountAsync(courseId);
            return Ok(new { CourseId = courseId, Count = cnt });
        }

    }
}
