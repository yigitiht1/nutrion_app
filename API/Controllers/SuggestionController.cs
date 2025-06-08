using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuggestionController : ControllerBase
    {
        private readonly ISuggestionService _suggestionService;

        public SuggestionController(ISuggestionService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetSuggestion(int userId)
        {
            var suggestion = await _suggestionService.GetSuggestionAsync(userId);
            return Ok(new { suggestion });
        }
    }
}