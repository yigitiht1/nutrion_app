using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

public interface IMealPlanService
{
    Task<MealPlan> CreateMealPlanForUserAsync(int userId);
}
