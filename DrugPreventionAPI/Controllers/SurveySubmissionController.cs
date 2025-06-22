using System.Security.Claims;
using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/surveys/{surveyId:int}/submissions")]
    public class SurveySubmissionController : ControllerBase
    {
        private readonly ISurveySubmissionRepository _surveySubmissionRepository;
        private readonly IMapper _mapper;
        public SurveySubmissionController(ISurveySubmissionRepository surveySubmissionRepository, IMapper mapper)
        {
            _mapper = mapper;
            _surveySubmissionRepository = surveySubmissionRepository;
        }

        [HttpGet("get-questions")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestions(int surveyId)
        {
            var dtos = await _surveySubmissionRepository.GetQuestionsForSubmissionAsync(surveyId);
            return Ok(dtos);
        }

        [HttpPost("submit")]
        [Authorize]
        public async Task<IActionResult> Submit(int surveyId, [FromBody] IEnumerable<SurveyAnswerDTO> answers)
        {
            var memberId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var sub = await _surveySubmissionRepository.SubmitAsync(surveyId, memberId, answers);
            var dto = _mapper.Map<SurveySubmissionReadDTO>(sub);
            return CreatedAtAction(nameof(GetById), new { surveyId, submissionId = sub.Id }, dto);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Consultant, Staff")]
        public async Task<IActionResult> GetBySurvey(int surveyId)
        {
            var submissions = await _surveySubmissionRepository.GetBySurveyAsync(surveyId);
            var dtos = _mapper.Map<IEnumerable<SurveySubmissionReadDTO>>(submissions);
            return Ok(dtos);
        }

        [HttpGet("users/{memberId:int}/submissions")]
        [Authorize]
        public async Task<IActionResult> GetByUser(int memberId)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idClaim == null ||
                (int.Parse(idClaim) != memberId &&
                 !User.IsInRole("Manager") &&
                 !User.IsInRole("Consultant")))
            {
                return Forbid();
            }

            var submissions = await _surveySubmissionRepository.GetByUserAsync(memberId);
            if (submissions == null || !submissions.Any())
                return NotFound("No submissions found for this user.");

            var dtos = _mapper.Map<IEnumerable<SurveySubmissionReadDTO>>(submissions);
            return Ok(dtos);
        }

        [HttpGet("{submissionId:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int surveyId, int submissionId)
        {
            var submission = await _surveySubmissionRepository.GetByIdAsync(submissionId);
            if (submission == null || submission.SurveyId != surveyId)
            {
                return NotFound($"Submission with ID {submissionId} not found for survey {surveyId}");
            }
            var dto = _mapper.Map<SurveySubmissionDetailDTO>(submission);
            return Ok(dto); 
        }
    }
}
