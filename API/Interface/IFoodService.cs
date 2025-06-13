using API.Models;

public interface IFoodService
{
    Task<List<FoodDto>> GetAllFoodsAsync(); // DTO dönmeli
    Task<List<FoodDto>> GetFoodsByMealTypeAsync(MealType mealType);
    Task AddFoodAsync(FoodDto foodDto);
    Task<bool> DeleteFoodAsync(string name);
}