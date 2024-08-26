using BotKernel.Common;
using BotKernel.Models;
using BotKernel.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BotKernel.Controllers
{
    [Route("employee")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly EmployeeContactsRepository _repository;

        public ContactsController(EmployeeContactsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetCandidateListByKeyword([FromQuery] string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword) || keyword.Length <= 1)
            {
                return BadRequest(new ErrorResponseMessage
                {
                    Message = "關鍵字數必須大於 1",
                    ErrorCode = 1
                });
            }

            var candidateList = await _repository.GetUserProfileListByKeywordAsync(keyword);

            if (candidateList.Count() > 10)
            {
                return BadRequest(new ErrorResponseMessage
                {
                    Message = "取得資料超過 10 筆",
                    ErrorCode = 2
                });
            }

            return Ok(candidateList);
        }

        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetProfileById(string id)
        {
            var profile = await _repository.GetUserProfileByIdAsync(id);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpGet("{id}/contact")]
        public async Task<IActionResult> GetCandidateById(string id)
        {
            var candidate = await _repository.GetContactByIdAsync(id);

            if (candidate == null)
            {
                return NotFound();
            }

            return Ok(candidate);
        }
    }
}
