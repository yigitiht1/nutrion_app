public interface IMealPlanService
{
    Task CreateMealPlanAsync(int userId, MealPlanDto dto);
    Task<List<MealPlanDto>> GetMealPlansByUserAsync(int userId);
    Task DeleteMealPlanAsync(int userId, int mealPlanId);
}