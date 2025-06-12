using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.DTOs;
using API.Models;

[ApiController]
[Route("api/[controller]")]
public class FoodController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFoods()
    {
        var foods = await _foodService.GetAllFoodsAsync();
        return Ok(foods);
    }

    // Öğün tipine göre yiyecekleri getir


    [HttpPost]
    public async Task<IActionResult> AddFood(FoodDto foodDto)
    {
        await _foodService.AddFoodAsync(foodDto);
        return Ok(new { message = "Yemek eklendi." });
    }
    [HttpGet("meal/{mealType}")]
    public async Task<IActionResult> GetFoodsByMealType(string mealType)
    {
        if (!Enum.TryParse<MealType>(mealType, true, out var parsedMealType))
        {
            return BadRequest("Geçersiz meal type.");
        }

        var foods = await _foodService.GetFoodsByMealTypeAsync(parsedMealType);
        return Ok(foods);
    }
}