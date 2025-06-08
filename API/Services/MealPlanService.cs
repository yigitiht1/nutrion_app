using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;

public class MealPlanService : IMealPlanService
{
    private readonly AppDbContext _context;

    public MealPlanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MealPlan> CreateMealPlanAsync(MealPlanDto dto)
    {
        var mealPlan = new MealPlan
        {
            UserId = dto.UserId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Items = dto.Items.Select(i => new MealPlanItem
            {
                Date = i.Date,
                MealType = i.MealType,
                FoodId = i.FoodId,
                Quantity = i.Quantity
            }).ToList()
        };

        _context.MealPlans.Add(mealPlan);
        await _context.SaveChangesAsync();

        return mealPlan;
    }

    public async Task<MealPlan?> GetMealPlanByUserIdAsync(int userId)
    {
        return await _context.MealPlans
            .Include(mp => mp.Items)
            .ThenInclude(i => i.Food)
            .FirstOrDefaultAsync(mp => mp.UserId == userId);
    }

    public async Task<bool> DeleteMealPlanAsync(int id)
    {
        var mealPlan = await _context.MealPlans.FindAsync(id);
        if (mealPlan == null) return false;

        _context.MealPlans.Remove(mealPlan);
        await _context.SaveChangesAsync();
        return true;
    }
}