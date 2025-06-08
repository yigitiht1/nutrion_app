using API.DTOs;
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

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateMealPlan([FromBody] MealPlanDto dto)
    {
        try
        {
            var meals = await _mealPlanService.CreateMealPlanAsync(dto);
            return Ok(meals);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Sunucu hatası: {ex.Message}");
        }
    }
}