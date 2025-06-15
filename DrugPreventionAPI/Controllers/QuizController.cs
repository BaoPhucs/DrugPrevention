using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId:int}/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IMapper _mapper;
        public QuizController(IQuizRepository quizRepository, IMapper mapper)
        {
            _mapper = mapper;
            _quizRepository = quizRepository;
        }

        [HttpGet("get-quiz")]
        [Authorize]
        public async Task<IActionResult> GetQuizQuestions(int courseId, [FromQuery] int count = 10)
        {
            if (count <= 0) return BadRequest("Count must be greater than 0");
            var questions = await _quizRepository.GetQuizQuestionsAsync(courseId, count);
            if (questions == null || !questions.Any())
            {
                return NotFound("No quiz questions found for this course");
            }
            return Ok(_mapper.Map<IEnumerable<QuizQuestionDTO>>(questions));
        }

        [HttpPost("submit-answers")]
        [Authorize]
        public async Task<IActionResult> SubmitQuizAnswers(int courseId, [FromBody] IEnumerable<QuizAnswerDTO> answers)
        {
            if (answers == null || !answers.Any())
            {
                return BadRequest("No answers provided");
            }
            var memberId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var sub = await _quizRepository.SubmitQuizAsync(courseId, memberId, answers);
            // Map về DetailDTO
            var dto = _mapper.Map<QuizSubmissionDetailDTO>(sub);
            return Ok(dto);
        }

        [HttpGet("submissions")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var list = await _quizRepository.GetSubmissionsByCourseAsync(courseId);
            var dtos = _mapper.Map<IEnumerable<QuizSubmissionReadDTO>>(list);
            return Ok(dtos);
        }


        [HttpGet("~/api/users/{memberId:int}/quiz/submissions")]
        [Authorize]
        public async Task<IActionResult> GetByUser(int memberId)
        {
            // chỉ user chính nó hoặc Admin/Consultant được xem
            var cur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (cur != memberId && !User.IsInRole("Admin,Manager,Consultant"))
                return Forbid();

            var list = await _quizRepository.GetSubmissionsByUserAsync(memberId);
            var dtos = _mapper.Map<IEnumerable<QuizSubmissionReadDTO>>(list);
            return Ok(dtos);
        }

        [HttpGet("~/api/quiz/submissions/{submissionId:int}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(int submissionId)
        {
            var sub = await _quizRepository.GetSubmissionByIdAsync(submissionId);
            // đảm bảo user có quyền
            var cur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (sub.MemberId != cur && !User.IsInRole("Admin,Manager,Consultant"))
                return Forbid();

            var dto = _mapper.Map<QuizSubmissionDetailDTO>(sub);
            return Ok(dto);
        }
    }
}
