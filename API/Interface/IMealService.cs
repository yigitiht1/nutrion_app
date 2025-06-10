using API.DTOs;
using API.Models;

public interface IMealService
{
    Task AddMealAsync(MealDto dto);
    Task<List<Meal>> GetMealsByUserAsync(int userId);
}