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

    [HttpPost("create/{userId}")]
    public async Task<IActionResult> CreateMealPlan(int userId)
    {
        try
        {
            var mealPlan = await _mealPlanService.CreateMealPlanForUserAsync(userId);
            return Ok(mealPlan);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}