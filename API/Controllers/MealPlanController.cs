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
    public async Task<ActionResult<MealPlan>> Create([FromBody] MealPlanDto dto)
    {
        try
        {
            var result = await _mealPlanService.CreateMealPlanAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<MealPlan>> GetByUserId(int userId)
    {
        var result = await _mealPlanService.GetMealPlanByUserIdAsync(userId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _mealPlanService.DeleteMealPlanAsync(id);
        return success ? Ok() : NotFound();
    }
}