using API.DTOs;
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IMealPlanService
{
    Task<MealPlan> CreateMealPlanAsync(MealPlanDto dto);
    Task<MealPlan?> GetMealPlanByUserIdAsync(int userId);
    Task<bool> DeleteMealPlanAsync(int id);
}