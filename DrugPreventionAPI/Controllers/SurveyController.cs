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
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IMapper _mapper;
        public SurveyController(ISurveyRepository surveyRepository, IMapper mapper)
        {
            _mapper = mapper;
            _surveyRepository = surveyRepository;
        }

        [HttpGet("get-all-survey-questions")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> GetAllSurveyQuestions()
        {
            var questions = await _surveyRepository.GetAllAsync();
            if (questions == null || !questions.Any())
            {
                return NotFound("No survey questions found");
            }
            return Ok(_mapper.Map<IEnumerable<SurveyDTO>>(questions));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> Get(int id)
        {
            var s = await _surveyRepository.GetByIdAsync(id);
            if (s == null) return NotFound();
            return Ok(_mapper.Map<SurveyDTO>(s));
        }

        [HttpPost("create-survey")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> CreateSurvey([FromBody] CreateSurveyDTO surveyDto)
        {
            var entity = _mapper.Map<Survey>(surveyDto);
            if (!await _surveyRepository.CreateAsync(entity))
                return StatusCode(500);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<SurveyDTO>(entity));
        }

        [HttpPut("update-survey/{id:int}")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> UpdateSurvey(int id, [FromBody] CreateSurveyDTO surveyDto)
        {
            var existing = await _surveyRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(surveyDto, existing);
            if (!await _surveyRepository.UpdateAsync(existing))
                return StatusCode(500);
            return NoContent();
        }

        [HttpDelete("delete-survey/{id:int}")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            if (!await _surveyRepository.DeleteAsync(id)) return NotFound();
            return NoContent();
        }
    }
}
