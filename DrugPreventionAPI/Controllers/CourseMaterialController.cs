using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId:int}/[controller]")]
    public class CourseMaterialController : ControllerBase
    {
        private readonly ICourseMaterialRepository _courseMaterialRepository;
        private readonly IMapper _mapper;
        public CourseMaterialController(ICourseMaterialRepository courseMaterialRepository, IMapper mapper)
        {
            _courseMaterialRepository = courseMaterialRepository;
            _mapper = mapper;
        }

        [HttpGet("get-all-materials")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCourseMaterials()
        {
            var materials = await _courseMaterialRepository.GetAllCourseMaterials();
            if (materials == null || !materials.Any())
            {
                return NotFound("No materials found");
            }
            // Map the materials to a DTO if necessary
            var materialDtos = _mapper.Map<IEnumerable<CourseMaterialDTO>>(materials);
            return Ok(materialDtos);
        }

        [HttpGet("get-materials-of-course")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseMaterials(int courseId)
        {
            var materials = await _courseMaterialRepository.GetCourseMaterialsByCourseAsync(courseId);
            if (materials == null || !materials.Any())
            {
                return NotFound("No materials found for this course");
            }
            // Map the materials to a DTO if necessary
            var materialDtos = _mapper.Map<IEnumerable<CourseMaterialDTO>>(materials);
            return Ok(materialDtos);
        }

        [HttpGet("get-material/{materialId:int}", Name = "GetCourseMaterialById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseMaterialById(int courseId, int materialId)
        {
            var material = await _courseMaterialRepository.GetCourseMaterialByIdAsync(materialId);
            if (material == null || material.CourseId != courseId)
            {
                return NotFound($"Material with ID {materialId} not found for course {courseId}");
            }
            // Map the material to a DTO if necessary
            var materialDto = _mapper.Map<CourseMaterialDTO>(material);
            return Ok(materialDto);
        }

        [HttpGet("get-materials-by-type/{type}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseMaterialsByType(int courseId, string type)
        {
            var materials = await _courseMaterialRepository.GetCourseMaterialsByTypeAsync(type);
            if (materials == null || !materials.Any())
            {
                return NotFound($"No materials found of type {type} for course {courseId}");
            }
            // Map the materials to a DTO if necessary
            var materialDtos = _mapper.Map<IEnumerable<CourseMaterialDTO>>(materials);
            return Ok(materialDtos);
        }

        [HttpPost("add-material")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> AddCourseMaterial(int courseId, [FromBody] CourseMaterialDTO courseMaterialDto)
        {
            if (courseMaterialDto == null)
            {
                return BadRequest("Invalid material data");
            }
            var courseMaterial = _mapper.Map<CourseMaterial>(courseMaterialDto);
            var result = await _courseMaterialRepository.AddCourseMaterialAsync(courseId, courseMaterial);
            if (result == null)
            {
                return NotFound($"Course {courseId} not found");
            }

            var readDto = _mapper.Map<CourseMaterialReadDTO>(result);

            return CreatedAtRoute(routeName: "GetCourseMaterialById",
                                  routeValues: new { courseId = courseId, materialId = result.Id },
                                  value: readDto); // Return 201 Created with the new material
        }

        [HttpPut("update-material/{materialId:int}")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> UpdateCourseMaterial(int courseId, int materialId, [FromBody] CourseMaterialDTO courseMaterialDto)
        {
            if (courseMaterialDto == null)
            {
                return BadRequest("Invalid material data");
            }
            var courseMaterial = _mapper.Map<CourseMaterial>(courseMaterialDto);
            var result = await _courseMaterialRepository.UpdateCourseMaterialAsync(courseId, materialId, courseMaterial);
            if (!result)
            {
                return NotFound($"Material with ID {materialId} not found for course {courseId}");
            }
            return NoContent(); // Return 204 No Content on successful update
        }

        [HttpDelete("delete-material/{materialId:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteCourseMaterial(int courseId, int materialId)
        {
            var result = await _courseMaterialRepository.DeleteCourseMaterialAsync(courseId, materialId);
            if (!result)
            {
                return NotFound($"Material with ID {materialId} not found for course {courseId}");
            }
            return NoContent(); // Return 204 No Content on successful deletion
        }
    }
}
