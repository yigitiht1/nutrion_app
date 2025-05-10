using API.DTOs;
using API.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;

public class MealService : IMealService
{
    private readonly AppDbContext _context;

    public MealService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddMealAsync(MealDto dto)
    {
        var foods = await _context.Foods
            .Where(f => dto.FoodIds.Contains(f.Id))
            .ToListAsync();

        var meal = new Meal
        {
            UserId = dto.UserId,
            MealType = dto.MealType,
            Date = dto.Date,
            Foods = foods
        };

        _context.Meals.Add(meal);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Meal>> GetMealsByUserAsync(int userId)
    {
        return await _context.Meals
            .Include(m => m.Foods)
            .Where(m => m.UserId == userId)
            .ToListAsync();
    }
}