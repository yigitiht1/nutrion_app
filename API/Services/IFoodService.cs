using API.Models;

public interface IFoodService
{
    Task<List<FoodDto>> GetAllFoodsAsync(); // DTO dönmeli
    // IFoodService implementasyonu zaten yukarıdaki metod
    public Task<List<FoodDto>> GetFoodsByMealTypeAsync(MealType mealType);
    Task AddFoodAsync(FoodDto foodDto);
}