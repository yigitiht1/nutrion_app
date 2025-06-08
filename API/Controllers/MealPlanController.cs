using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class MealPlanController : ControllerBase
{
    private readonly MealPlanService _mealPlanService;

    public MealPlanController(MealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateMealPlan([FromBody] CreateMealPlanRequest request)
    {
        try
        {
            var mealPlan = await _mealPlanService.CreateMealPlanAsync(request.UserId, request.StartDate, request.EndDate);
            return Ok(mealPlan);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class CreateMealPlanRequest
{
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}