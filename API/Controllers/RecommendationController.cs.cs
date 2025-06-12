using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly ICalorieRecommendationService _recommendationService;
        private readonly AppDbContext _context;

        public RecommendationController(ICalorieRecommendationService recommendationService, AppDbContext context)
        {
            _recommendationService = recommendationService;
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<RecommendationDto>> GetRecommendation(int userId)
        {
            var today = DateTime.UtcNow.Date;

            var meals = await _context.Meals
                .Include(m => m.MealItems)
                .ThenInclude(mi => mi.Food)
                .Where(m => m.UserId == userId && m.Date.Date == today)
                .ToListAsync();

            int totalCaloriesToday = (int)meals.SelectMany(m => m.MealItems).Sum(i => i.Food.Calories * i.Quantity);

            var result = await _recommendationService.GetRecommendationForUserAsync(userId, totalCaloriesToday);

            if (result == null)
                return NotFound("Kullanıcı profili bulunamadı.");

            return Ok(result);
        }
    }
}