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
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;
        public QuestionController(IQuestionRepository questionRepository, IMapper mapper)
        {
            _mapper = mapper;
            _questionRepository = questionRepository;
        }

        [HttpGet("get-all-questions")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> GetAllQuestions()
        {
            var questions = await _questionRepository.GetAllAsync();
            if (questions == null || !questions.Any())
            {
                return NotFound("No questions found");
            }
            // Optionally map to a DTO if needed
            var questionDTOs = _mapper.Map<IEnumerable<QuestionDTO>>(questions);
            return Ok(questionDTOs);
        }


        [HttpGet("get-question/{id}")]
        [Authorize(Roles = "Manager, Consultant, Staff, Member")]
        public async Task<IActionResult> GetQuestion(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
            {
                return NotFound($"Question with ID {id} not found");
            }
            // Optionally map to a DTO if needed
            var questionDTO = _mapper.Map<QuestionDTO>(question);
            return Ok(questionDTO);
        }


        //[HttpPost("create-question")]
        //[Authorize(Roles = "Manager, Consultant")]
        //public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionDTO dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var entity = _mapper.Map<QuestionBank>(dto);
        //    var created = await _questionRepository.CreateAsync(entity);
        //    if (!created)
        //        return StatusCode(500, "Error creating question");

        //    // Lấy lại question có options rỗng
        //    var resultDto = _mapper.Map<QuestionDTO>(entity);
        //    return CreatedAtAction(nameof(GetQuestion), new { id = resultDto.Id }, resultDto);
        //}


        // Tạo 1 câu hỏi
        [HttpPost("create-question")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> Create([FromBody] CreateQuestionDTO dto)
        {
            var entity = _mapper.Map<QuestionBank>(dto);
            var created = await _questionRepository.CreateAsync(entity);
            var result = _mapper.Map<QuestionDTO>(created);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // (tùy chọn) tạo nhiều câu hỏi
        [HttpPost("create-questions")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> CreateBatch([FromBody] IEnumerable<CreateQuestionDTO> dtos)
        {
            var entities = _mapper.Map<IEnumerable<QuestionBank>>(dtos);
            var created = await _questionRepository.CreateRangeAsync(entities);
            var results = _mapper.Map<IEnumerable<QuestionDTO>>(created);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var q = await _questionRepository.GetByIdAsync(id);
            if (q == null) return NotFound();
            return Ok(_mapper.Map<QuestionDTO>(q));
        }











        [HttpPut("update-question/{id}")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] CreateQuestionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            var existing = await _questionRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Question with ID {id} not found");

            
            existing.QuestionText = dto.QuestionText;
            existing.Level = dto.Level;
           

            var updated = await _questionRepository.UpdateAsync(existing);
            if (!updated)
                return StatusCode(500, "An error occurred while updating the question");

            return NoContent();
        }

        [HttpDelete("delete-question/{id}")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var existingQuestion = await _questionRepository.GetByIdAsync(id);
            if (existingQuestion == null)
            {
                return NotFound($"Question with ID {id} not found");
            }
            var deleted = await _questionRepository.DeleteAsync(id);
            if (!deleted)
            {
                return StatusCode(500, "An error occurred while deleting the question");
            }
            return NoContent();
        }


        [HttpGet("{questionId:int}/get-options")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> GetOptions(int questionId)
        {
            var options = await _questionRepository.GetOptionsAsync(questionId);
            if (options == null || !options.Any())
            {
                return NotFound($"No options found for question ID {questionId}");
            }
            var dtos = _mapper.Map<IEnumerable<OptionDTO>>(options);
            return Ok(dtos);
        }

        [HttpPost("{questionId:int}/add-options")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> AddOption(int questionId, [FromBody] IEnumerable<CreateOptionDTO> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest("No options provided");

            var entities = dtos.Select(dto => _mapper.Map<QuestionOption>(dto)).ToList();
            var added = await _questionRepository.AddOptionsAsync(questionId, entities);
            if (!added)
                return StatusCode(500, "Error adding options");

            var resultDtos = _mapper.Map<IEnumerable<OptionDTO>>(entities);
            return Ok(resultDtos);
        }

        [HttpPut("{questionId:int}/update-options/{optionId:int}")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> UpdateOption(int questionId, int optionId, [FromBody] CreateOptionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid option data");
            }
            
            var existingOption = await _questionRepository.GetOptionByIdAsync(questionId, optionId);
            if (existingOption == null)
            {
                return NotFound($"Option with ID {optionId} not found for question ID {questionId}");
            }

            existingOption.OptionText = dto.OptionText;
            existingOption.ScoreValue = dto.ScoreValue;

            var updated = await _questionRepository.UpdateOptionAsync(questionId, existingOption);
            if (!updated)
            {
                return StatusCode(500, "An error occurred while updating the option");
            }
            return NoContent();
        }

        [HttpDelete("{questionId:int}/delete-options/{optionId:int}")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> DeleteOption(int questionId, int optionId)
        {
            var existingOption = await _questionRepository.GetOptionByIdAsync(questionId, optionId);
            if (existingOption == null)
            {
                return NotFound($"Option with ID {optionId} not found for question ID {questionId}");
            }
            var deleted = await _questionRepository.DeleteOptionAsync(questionId, optionId);
            if (!deleted)
            {
                return StatusCode(500, "An error occurred while deleting the option");
            }
            return NoContent();
        }
    }
}
