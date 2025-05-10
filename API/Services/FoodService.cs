using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Data;
public class FoodService : IFoodService
{
    private readonly AppDbContext _context;
    public FoodService(AppDbContext context) => _context = context;

    public async Task AddFoodAsync(FoodDto dto)
    {
        var food = new Food
        {
            Name = dto.Name,
            Calories = dto.Calories,
            Protein = dto.Protein,
            Carbs = dto.Carbs,
            Fat = dto.Fat
        };
        _context.Foods.Add(food);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Food>> GetAllFoodsAsync()
    {
        return await _context.Foods.ToListAsync();
    }
}