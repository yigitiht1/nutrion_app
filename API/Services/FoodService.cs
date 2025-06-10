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
    var food = new Food
    {
        Name = dto.Name,
        Calories = dto.Calories,
        Protein = dto.Protein,
        Carbs = dto.Carbs,
        Fat = dto.Fat,
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
    }public async Task<List<Food>> GetFoodsByMealTypeAsync(MealType mealType)
{
    try
    {
        return await _context.FoodMealTypes
            .Include(fmt => fmt.Food)
            .Where(fmt => fmt.MealType == mealType)
            .Select(fmt => fmt.Food)
            .ToListAsync();
    }
    catch (Exception ex)
    {
        // Log kaydı veya konsola yazdır
        Console.WriteLine($"[ERROR] GetFoodsByMealTypeAsync: {ex.Message}");
        throw; // hata fırlatmaya devam et
    }
}

    Task<List<FoodDto>> IFoodService.GetFoodsByMealTypeAsync(MealType mealType)
    {
        throw new NotImplementedException();
    }
}