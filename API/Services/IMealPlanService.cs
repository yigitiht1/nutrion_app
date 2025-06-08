using API.DTOs;
using API.Models;

public interface IMealPlanService
{
    Task<List<Meal>> CreateMealPlanAsync(MealPlanDto dto);
}