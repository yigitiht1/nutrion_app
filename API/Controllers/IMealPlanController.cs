using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MealPlanController : ControllerBase
{
    private readonly IMealPlanService _mealPlanService;

    public MealPlanController(IMealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMealPlan([FromBody] MealPlanDto dto)
    {
        try
        {
            var meals = await _mealPlanService.CreateMealPlanAsync(dto);
            return Ok(meals);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetMealPlan(int userId)
    {
        var meals = await _mealPlanService.GetMealPlanByUserIdAsync(userId);
        if (meals == null || !meals.Any())
            return NotFound("Kullanıcı için diyet listesi bulunamadı.");

        return Ok(meals);
    }
}