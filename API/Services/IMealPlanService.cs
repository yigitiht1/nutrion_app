using API.DTOs;
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IMealPlanService
{
    Task<List<Meal>> CreateMealPlanAsync(MealPlanDto dto);
    Task<List<Meal>?> GetMealPlanByUserIdAsync(int userId);
}