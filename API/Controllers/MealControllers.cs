using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Services;
using API.Data;
using API.Models;

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
                Foods = m.MealItems.Select(i => new
                {
                    i.Food.Name,
                    i.Food.Calories,
                    i.Food.Protein,
                    i.Food.Fat,
                    i.Food.Carbs,
                    i.Quantity
                }),
                TotalMealCalories = m.MealItems.Sum(i => i.Food.Calories * i.Quantity),
                TotalMealProtein = m.MealItems.Sum(i => i.Food.Protein * i.Quantity),
                TotalMealCarbs = m.MealItems.Sum(i => i.Food.Carbs * i.Quantity),
                TotalMealFat = m.MealItems.Sum(i => i.Food.Fat * i.Quantity),
            });

            int totalCalories = (int)meals.SelectMany(m => m.MealItems).Sum(i => i.Food.Calories * i.Quantity);
            double totalProtein = meals.SelectMany(m => m.MealItems).Sum(i => i.Food.Protein * i.Quantity);
            double totalCarbs = meals.SelectMany(m => m.MealItems).Sum(i => i.Food.Carbs * i.Quantity);
            double totalFat = meals.SelectMany(m => m.MealItems).Sum(i => i.Food.Fat * i.Quantity);

            return Ok(new
            {
                Meals = response,
                TotalCaloriesToday = totalCalories,
                TotalProteinToday = totalProtein,
                TotalCarbsToday = totalCarbs,
                TotalFatToday = totalFat
            });
        }
        [HttpGet("daily/{userId}/{date}")]
        public async Task<IActionResult> GetMealsByDate(int userId, DateTime date)
        {
            var meals = await _context.Meals
                .Include(m => m.MealItems)
                .ThenInclude(mi => mi.Food)
                .Where(m => m.UserId == userId && m.Date.Date == date.Date)
                .ToListAsync();

            var response = meals.Select(m => new
            {
                m.MealType,
                Foods = m.MealItems.Select(i => new
                {
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

            return Ok(new
            {
                Meals = response,
                TotalCalories = totalCalories
            });
        }
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetMealHistory(int userId, DateTime startDate, DateTime endDate)
        {
            var meals = await _context.Meals
                .Include(m => m.MealItems)
                .ThenInclude(mi => mi.Food)
                .Where(m => m.UserId == userId && m.Date.Date >= startDate.Date && m.Date.Date <= endDate.Date)
                .ToListAsync();

            var groupedByDate = meals
                .GroupBy(m => m.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Meals = g.Select(m => new
                    {
                        m.MealType,
                        Foods = m.MealItems.Select(i => new
                        {
                            i.Food.Name,
                            i.Food.Calories,
                            i.Food.Protein,
                            i.Food.Fat,
                            i.Food.Carbs,
                            i.Quantity
                        }),
                        TotalMealCalories = m.MealItems.Sum(i => i.Food.Calories * i.Quantity)
                    }),
                    TotalCalories = g.SelectMany(m => m.MealItems).Sum(i => i.Food.Calories * i.Quantity)
                });

            return Ok(groupedByDate);
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
        [HttpPost]
        public async Task<IActionResult> AddMeal(MealDto mealDto)
        {
            await _mealService.AddMealAsync(mealDto);
            return Ok(new { message = "Öğün eklendi." });
        }

    }
}
