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

        [HttpGet("get-courses-by-createById/{createById}")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> GetCoursesByCreateById(int createById)
        {
            var courses = await _courseRepository.GetCoursesByCreatedByIdAsync(createById);
            if (courses == null || !courses.Any())
            {
                return NotFound($"No courses found created by user with ID {createById}");
            }
            // Map the courses to CourseDTOs
            var courseDtos = _mapper.Map<IEnumerable<CourseDTO>>(courses);
            return Ok(courseDtos);
        }

        [HttpGet("get-courses-by-status/{status}")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> GetCoursesByStatus(string status)
        {
            var courses = await _courseRepository.GetCoursesByStatusAsync(status);
            if (courses == null || !courses.Any())
            {
                return NotFound($"No courses found with status {status}");
            }
            // Map the courses to CourseDTOs
            var courseDtos = _mapper.Map<IEnumerable<CourseDTO>>(courses);
            return Ok(courseDtos);
        }

        [HttpPost("create-course")]
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDTO courseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var course = _mapper.Map<Course>(courseDto);
            course.CreatedById = CurrentUserId; // Gán ID người dùng hiện tại
            course.CreatedDate = DateTime.UtcNow; // Gán ngày tạo
            course.Status = "Pending"; // Mặc định trạng thái là Pending
            course.WorkflowState = "Draft";
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
            if (!ModelState.IsValid)
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

        [HttpPost("{courseId}/submit-to-staff")]
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> SubmitToStaff(int courseId)
        {
            var ok = await _courseRepository.SubmitToStaffAsync(courseId, CurrentUserId);
            if (!ok) return BadRequest("Không hợp lệ hoặc khóa học không ở trạng thái Draft.");
            return NoContent();
        }

        [HttpPost("{courseId}/submit-to-manager")]
        [Authorize(Roles = "Consultant, Staff")]
        public async Task<IActionResult> SubmitToManager(int courseId)
        {
            var ok = await _courseRepository.SubmitToManagerAsync(courseId, CurrentUserId);
            if (!ok) return BadRequest("Không hợp lệ hoặc khóa học chưa được gửi tới Staff.");
            return NoContent();
        }

        [HttpPost("{courseId}/published")]
        [Authorize(Roles = "Manager, Staff")]
        public async Task<IActionResult> PublishCourse(int courseId)
        {
            var ok = await _courseRepository.PublicCourse(courseId, CurrentUserId);
            if (!ok) return BadRequest("Không hợp lệ hoặc khóa học chưa được phê duyệt.");
            return NoContent();
        }

        [HttpPut("{id:int}/schedule-publish")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> SchedulePublish(int id, [FromBody] SchedulePublishDTO dto)
        {
            if (dto.PublishAt <= DateTime.UtcNow)
                return BadRequest("PublishAt phải ở tương lai.");

            var ok = await _courseRepository.SchedulePublishAsync(id, dto.PublishAt);
            if (!ok)
                return BadRequest("Không thể lên lịch (chưa Approved hoặc Course không tồn tại).");

            return NoContent();
        }

        [HttpPost("{id:int}/publish")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Publish(int id)
        {
            var ok = await _courseRepository.PublishIfDueAsync(id);
            if (!ok) return BadRequest("Khóa học chưa đến hạn hoặc không tồn tại.");
            return NoContent();
        }

        // 2) Publish tất cả khóa học đến hạn (cron hoặc frontend có thể gọi)
        [HttpPost("publish-due")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> PublishDue()
        {
            var count = await _courseRepository.PublishAllDueAsync();
            return Ok(new { PublishedCount = count });
        }
    }
}
