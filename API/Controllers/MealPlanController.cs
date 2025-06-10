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

    // Kullanıcı ID'yi route parametresi olarak alıyoruz
    [HttpPost("create/{userId}")]
    public async Task<IActionResult> CreateMealPlan(int userId, [FromBody] MealPlanDto dto)
    {
        await _mealPlanService.CreateMealPlanAsync(userId, dto);
        return Ok("Meal plan created.");
    }

    [HttpGet("user/{userId}")] 
    public async Task<IActionResult> GetUserMealPlans(int userId)
    {
        var plans = await _mealPlanService.GetMealPlansByUserAsync(userId);
        return Ok(plans);
    }

    [HttpDelete("{userId}/{id}")]
    public async Task<IActionResult> DeleteMealPlan(int userId, int id)
    {
        await _mealPlanService.DeleteMealPlanAsync(userId, id);
        return Ok("Deleted");
    }
}