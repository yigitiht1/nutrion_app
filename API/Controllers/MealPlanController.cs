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
        var mealPlan = await _mealPlanService.CreateMealPlanAsync(dto);
        return CreatedAtAction(nameof(GetMealPlanByUserId), new { userId = mealPlan.UserId }, mealPlan);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetMealPlanByUserId(int userId)
    {
        var mealPlan = await _mealPlanService.GetMealPlanByUserIdAsync(userId);
        if (mealPlan == null) return NotFound();

        return Ok(mealPlan);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMealPlan(int id)
    {
        var deleted = await _mealPlanService.DeleteMealPlanAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}