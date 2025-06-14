using System.Security.Claims;
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
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        public CourseController(ICourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        [HttpGet("get-all-courses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            if (courses == null || !courses.Any())
            {
                return NotFound("No courses found");
            }

            // Map the courses to CourseDTOs
            var courseDtos = _mapper.Map<IEnumerable<CourseDTO>>(courses);
            return Ok(courseDtos);
        }

        [HttpGet("get-course/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound($"Course with ID {id} not found");
            }
            // Map the course to CourseDTO
            var courseDto = _mapper.Map<CourseDTO>(course);
            return Ok(courseDto);
        }

        [HttpGet("get-course-by-level/{level}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseByLevel(string level)
        {
            var course = await _courseRepository.GetCourseByLevelAsync(level);
            if (course == null)
            {
                return NotFound($"Course with level {level} not found");
            }
            // Map the course to CourseDTO
            var courseDtos = _mapper.Map<IEnumerable<CourseDTO>>(course);
            return Ok(courseDtos);
        }

        [HttpGet("get-courses-by-category/{category}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCoursesByCategory(string category)
        {
            var courses = await _courseRepository.GetCoursesByCategoryAsync(category);
            if (courses == null || !courses.Any())
            {
                return NotFound($"No courses found in category {category}");
            }
            // Map the courses to CourseDTOs
            var courseDtos = _mapper.Map<IEnumerable<CourseDTO>>(courses);
            return Ok(courseDtos);
        }

        [HttpPost("create-course")]
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDTO courseDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var course = _mapper.Map<Course>(courseDto);
            course.CreatedById = CurrentUserId; // Gán ID người dùng hiện tại
            course.CreatedDate = DateTime.UtcNow; // Gán ngày tạo
            course.Status = "Pending"; // Mặc định trạng thái là Pending

            var result = await _courseRepository.CreateCourseAsync(course);
            if (!result)
            {
                return StatusCode(500, "An error occurred while creating the course");
            }
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course); // Trả về 201 Created với thông tin khóa học mới tạo
        }


        [HttpDelete("delete-course/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseRepository.DeleteCourseAsync(id);
            if (!result)
            {
                return NotFound($"Course with ID {id} not found");
            }
            return NoContent(); // Trả về 204 No Content nếu xóa thành công
        }

        [HttpPut("update-course/{courseId}")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> UpdateCourse(int courseId, [FromBody] CourseDTO courseDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCourse = await _courseRepository.GetCourseByIdAsync(courseId);
            if (existingCourse == null)
            {
                return NotFound($"Course with ID {courseId} not found");
            }
            _mapper.Map(courseDto, existingCourse); // Cập nhật thông tin khóa học

            existingCourse.UpdateById = CurrentUserId; // Gán ID người dùng hiện tại
            existingCourse.UpdateDate = DateTime.UtcNow; // Cập nhật ngày sửa đổi
            existingCourse.Status = "Pending"; // Đặt lại trạng thái về Pending sau khi cập nhật

            var result = await _courseRepository.UpdateCourseAsync(existingCourse);
            if (!result)
            {
                return StatusCode(500, "An error occurred while updating the course");
            }
            return Ok(existingCourse); // Trả về 200 OK với thông tin khóa học đã cập nhật
        }


        [HttpPut("{id:int}/approve")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveCourse(int id)
        {
            var result = await _courseRepository.ApproveAsync(id);
            if (!result)
            {
                return NotFound($"Course with ID {id} not found or already approved");
            }
            return NoContent(); // Trả về 204 No Content nếu phê duyệt thành công
        }

        [HttpPut("{id:int}/reject")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> RejectCourse(int id, [FromBody] string reviewComments)
        {
            var result = await _courseRepository.RejectAsync(id, reviewComments);
            if (!result)
            {
                return NotFound($"Course with ID {id} not found or already rejected");
            }
            return NoContent(); // Trả về 204 No Content nếu từ chối thành công
        }
    }
}
