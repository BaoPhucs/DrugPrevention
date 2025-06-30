using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificateController : ControllerBase
    {

        private readonly ICertificateRepository _certRepo;
        private readonly IMapper _mapper;
        public CertificateController(ICertificateRepository certificateRepository, IMapper mapper)
        {
            _certRepo = certificateRepository;
            _mapper = mapper;
        }

        // Xem chứng chỉ theo user
        [HttpGet("member/{memberId:int}")]
        [Authorize]
        public async Task<IActionResult> GetByMember(int memberId)
        {
            var list = await _certRepo.GetByMemberAsync(memberId);
            return Ok(_mapper.Map<IEnumerable<CertificateDTO>>(list));
        }

        // Xem chứng chỉ theo course
        [HttpGet("course/{courseId:int}")]
        [Authorize(Roles = "Manager, Consultant")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var list = await _certRepo.GetByCourseAsync(courseId);
            return Ok(_mapper.Map<IEnumerable<CertificateDTO>>(list));
        }

        // Xem chi tiết 1 chứng chỉ
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var cert = await _certRepo.GetByIdAsync(id);
            if (cert == null) return NotFound();
            return Ok(_mapper.Map<CertificateDTO>(cert));
        }
    }
}
