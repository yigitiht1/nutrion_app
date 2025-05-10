using API.DTOs;
using API.Models;

public interface IFoodService
{
    Task AddFoodAsync(FoodDto dto);
    Task<List<Food>> GetAllFoodsAsync();
}