using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using API.DTOs;

namespace API.Controllers
{
    using global::API.Data;
    using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealController : ControllerBase
    {
        private readonly IMealService _mealService;
        private readonly AppDbContext _context;

        public MealController(IMealService mealService, AppDbContext context)
        {
            _mealService = mealService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddMeal(MealDto mealDto)
        {
            await _mealService.AddMealAsync(mealDto);
            return Ok(new { message = "Öğün eklendi." });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMealsByUser(int userId)
        {
            var meals = await _mealService.GetMealsByUserAsync(userId);
            return Ok(meals);
        }

        [HttpGet("daily/{userId}")]
        public async Task<IActionResult> GetDailyMeals(int userId)
        {
            var today = DateTime.UtcNow.Date;

            var meals = await _context.Meals
                .Include(m => m.MealItems)
                .ThenInclude(mi => mi.Food)
                .Where(m => m.UserId == userId && m.Date.Date == today)
                .ToListAsync();

            var response = meals.Select(m => new
            {
                m.MealType,
                Foods = m.MealItems.Select(i => new {
                    i.Food.Name,
                    i.Food.Calories,
                    i.Food.Protein,
                    i.Food.Fat,
                    i.Food.Carbs,
                    i.Quantity
                }),
                TotalMealCalories = m.MealItems.Sum(i => i.Food.Calories * i.Quantity)
            });

            int totalCalories = (int)meals.SelectMany(m => m.MealItems).Sum(i => i.Food.Calories * i.Quantity);

            return Ok(new {
                Meals = response,
                TotalCaloriesToday = totalCalories
            });
        }

        [HttpPost("recognize")]
        public async Task<IActionResult> RecognizeAndAssignFood([FromBody] RecognizeFoodDto dto)
        {
            var food = await _context.Foods.FirstOrDefaultAsync(f => f.Name.ToLower() == dto.FoodName.ToLower());
            if (food == null)
            {
                return NotFound("Yemek bulunamadı.");
            }

            var today = DateTime.UtcNow.Date;

            var existingMeal = await _context.Meals
                .Include(m => m.MealItems)
                .FirstOrDefaultAsync(m =>
                    m.UserId == dto.UserId &&
                    m.MealType == dto.MealType &&
                    m.Date.Date == today);

            if (existingMeal == null)
            {
                existingMeal = new Meal
                {
                    UserId = dto.UserId,
                    MealType = dto.MealType,
                    Date = today,
                    MealItems = new List<MealItem>()
                };
                _context.Meals.Add(existingMeal);
            }

            existingMeal.MealItems.Add(new MealItem
            {
                FoodId = food.Id,
                Quantity = dto.Quantity
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Yemek başarıyla öğüne eklendi.",
                food = new { food.Name, food.Calories }
            });
        }
    }
}
}