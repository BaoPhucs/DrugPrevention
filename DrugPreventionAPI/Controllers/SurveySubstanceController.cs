using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/surveys/{surveyId:int}/substances")]
    public class SurveySubstanceController : ControllerBase
    {
        private readonly ISurveySubstanceRepository _repo;
        private readonly IMapper _mapper;
        public SurveySubstanceController(ISurveySubstanceRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> GetAll(int surveyId)
        {
            var list = await _repo.GetBySurveyAsync(surveyId);
            return Ok(_mapper.Map<IEnumerable<SurveySubstanceDTO>>(list));
        }

        [HttpPost("create")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> Create(int surveyId, [FromBody] SurveySubstanceDTO dto)
        {
            var sb = _mapper.Map<SurveySubstance>(dto);
            sb.SurveyId = surveyId;
            var created = await _repo.CreateAsync(sb);
            return CreatedAtAction(nameof(GetAll), new { surveyId }, _mapper.Map<SurveySubstanceDTO>(created));
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> Update(int surveyId, int id, [FromBody] SurveySubstanceDTO dto)
        {
            if (id != dto.Id) return BadRequest();
            var sb = _mapper.Map<SurveySubstance>(dto);
            sb.SurveyId = surveyId;
            if (!await _repo.UpdateAsync(sb)) return NotFound();
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> Delete(int surveyId, int id)
        {
            if (!await _repo.DeleteAsync(id)) return NotFound();
            return NoContent();
        }
    }
}
