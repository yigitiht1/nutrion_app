using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using API.DTOs;

namespace API.Controllers
{
    [ApiController]
[Route("api/[controller]")]
public class MealController : ControllerBase
{
    private readonly IMealService _mealService;
    public MealController(IMealService mealService) => _mealService = mealService;

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
}
}