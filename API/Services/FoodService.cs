using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;

public class FoodService : IFoodService
{
    private readonly AppDbContext _context;

    public FoodService(AppDbContext context)
    {
        _context = context;
    }

public async Task AddFoodAsync(FoodDto dto)
{
    double factor = dto.PortionGrams / 100.0;

    var food = new Food
    {
        Name = dto.Name,
        Calories = dto.Calories * factor,
        Protein = dto.Protein * factor,
        Carbs = dto.Carbs * factor,
        Fat = dto.Fat * factor,
        CaloriesPer100g = dto.PortionGrams,
        FoodMealTypes = dto.MealTypes.Select(mt => new FoodMealType
        {
            MealType = mt
        }).ToList()
    };

    _context.Foods.Add(food);
    await _context.SaveChangesAsync();
}
   public async Task<List<FoodDto>> GetAllFoodsAsync()
    {
        var foods = await _context.Foods
            .Include(f => f.FoodMealTypes)
            .ToListAsync();

        return foods.Select(f => new FoodDto
        {
            Name = f.Name,
            Calories = f.Calories,
            Protein = f.Protein,
            Carbs = f.Carbs,
            Fat = f.Fat,
            MealTypes = f.FoodMealTypes.Select(m => m.MealType).ToList()
        }).ToList();
    }
    public async Task<List<Food>> GetFoodsByMealTypeAsync(MealType mealType)
    {
        return await _context.FoodMealTypes
            .Where(fmt => fmt.MealType == mealType)
            .Select(fmt => fmt.Food)
            .ToListAsync();
    }

    Task<List<FoodDto>> IFoodService.GetFoodsByMealTypeAsync(MealType mealType)
    {
        throw new NotImplementedException();
    }
}