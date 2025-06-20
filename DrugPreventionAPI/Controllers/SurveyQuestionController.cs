using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/surveys/{surveyId:int}/questions")]
    public class SurveyQuestionController : ControllerBase
    {
        private readonly ISurveyQuestionRepository _surveyQuestionRepository;
        private readonly IMapper _mapper;
        public SurveyQuestionController(ISurveyQuestionRepository surveyQuestionRepository, IMapper mapper)
        {
            _mapper = mapper;
            _surveyQuestionRepository = surveyQuestionRepository;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> GetAll(int surveyId)
        {
            var list = await _surveyQuestionRepository.GetBySurveyAsync(surveyId);
            return Ok(_mapper.Map<IEnumerable<SurveyQuestionDTO>>(list));
        }

        [HttpPost("create-question")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> Create(int surveyId, [FromBody] CreateSurveyQuestionDTO dto)
        {
            dto.SurveyId = surveyId;
            var entity = _mapper.Map<SurveyQuestion>(dto);
            if (!await _surveyQuestionRepository.CreateQuestionAsync(entity))
                return StatusCode(500);
            return CreatedAtAction(nameof(GetAll), new { surveyId }, _mapper.Map<SurveyQuestionDTO>(entity));
        }

        [HttpPut("update-question/{questionId:int}")]
        [Authorize(Roles = "Manager, Consultant, Staff  ")]
        public async Task<IActionResult> Update(int surveyId, int questionId, [FromBody] CreateSurveyQuestionDTO dto)
        {
            var q = await _surveyQuestionRepository.GetQuestionByIdAsync(questionId);
            if (q == null) return NotFound();
            _mapper.Map(dto, q);
            if (!await _surveyQuestionRepository.UpdateQuestionAsync(q)) return StatusCode(500);
            return NoContent();
        }

        [HttpDelete("delete-question/{questionId:int}")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> Delete(int surveyId, int questionId)
        {
            var q = await _surveyQuestionRepository.GetQuestionByIdAsync(questionId);
            if (q == null) return NotFound();
            if (!await _surveyQuestionRepository.DeleteQuestionAsync(questionId)) return StatusCode(500);
            return NoContent();
        }


        // --- SurveyOption ---
        [HttpGet("{questionId:int}/get-options")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> GetOptions(int surveyId, int questionId)
        {
            var options = await _surveyQuestionRepository.GetOptionsAsync(questionId);
            return Ok(_mapper.Map<IEnumerable<SurveyOptionDTO>>(options));
        }

        [HttpPost("{questionId:int}/add-options")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> AddOption(int surveyId, int questionId, [FromBody] IEnumerable<CreateSurveyOptionDTO> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest();

            // gán FK và map
            var entities = dtos.Select(dto =>
            {
                dto.QuestionId = questionId;
                return _mapper.Map<SurveyOption>(dto);
            }).ToList();

            var success = await _surveyQuestionRepository.CreateOptionAsync(entities);
            if (!success)
                return StatusCode(500, "Can not add options");

            var resultDtos = _mapper.Map<IEnumerable<SurveyOptionDTO>>(entities);
            return Ok(resultDtos);
        }

        [HttpPut("{questionId:int}/update-option/{optionId:int}")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> UpdateOption(int surveyId, int questionId, int optionId, [FromBody] CreateSurveyOptionDTO dto)
        {
            var option = await _surveyQuestionRepository.GetOptionByIdAsync(optionId);
            if (option == null) return NotFound();
            _mapper.Map(dto, option);
            if (!await _surveyQuestionRepository.UpdateOptionAsync(option)) return StatusCode(500);
            return NoContent();
        }

        [HttpDelete("{questionId:int}/delete-option/{optionId:int}")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> DeleteOption(int surveyId, int questionId, int optionId)
        {
            var option = await _surveyQuestionRepository.GetOptionByIdAsync(optionId);
            if (option == null) return NotFound();
            if (!await _surveyQuestionRepository.DeleteOptionAsync(optionId)) return StatusCode(500);
            return NoContent();
        }
    }
}
