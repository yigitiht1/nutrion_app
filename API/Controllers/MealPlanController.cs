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



    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserMealPlans(int userId)
    {
        var plans = await _mealPlanService.GetMealPlansByUserAsync(userId);
        return Ok(plans);
    }

    [HttpGet("user/{userId}/weekly-plan")]
    public async Task<IActionResult> GetWeeklyMealPlan(int userId)
    {
        var plan = await _mealPlanService.GenerateWeeklyMealPlanAsync(userId);
        return Ok(plan);
    }

    [HttpDelete("{userId}/{id}")]
    public async Task<IActionResult> DeleteMealPlan(int userId, int id)
    {
        await _mealPlanService.DeleteMealPlanAsync(userId, id);
        return Ok("Deleted");
    }
    
        [HttpPost("create/{userId}")]
    public async Task<IActionResult> CreateMealPlan(int userId, [FromBody] MealPlanDto dto)
    {
        await _mealPlanService.CreateMealPlanAsync(userId, dto);
        return Ok("Meal plan created.");
    }
}