using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using API.DTOs;

namespace API.Controllers
{
    [ApiController]
[Route("api/[controller]")]
public class FoodController : ControllerBase
{
    private readonly IFoodService _foodService;
    public FoodController(IFoodService foodService) => _foodService = foodService;
    
    [HttpGet]
    public async Task<IActionResult> GetAllFoods()
    {
        var foods = await _foodService.GetAllFoodsAsync();
        return Ok(foods);
    }
    [HttpPost]
    public async Task<IActionResult> AddFood(FoodDto foodDto)
    {
        await _foodService.AddFoodAsync(foodDto);
        return Ok(new { message = "Yemek eklendi." });
    }

   
}
}