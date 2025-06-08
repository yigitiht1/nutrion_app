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
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == dto.UserId);
        if (profile == null)
            throw new Exception("User profile not found.");

        double targetCalories = profile.CalculateRecommendedCalories();

        var foods = await _context.Foods.OrderBy(f => Guid.NewGuid()).ToListAsync();

        double accumulatedCalories = 0;
        List<MealPlanItem> selectedItems = new();

        foreach (var food in foods)
        {
            if (accumulatedCalories >= targetCalories)
                break;

            int quantity = 1; // miktarı isteğe göre artırılabilir
            accumulatedCalories += food.Calories * quantity;

            selectedItems.Add(new MealPlanItem
            {
                FoodId = food.Id,
                Quantity = quantity
            });
        }

        var mealPlan = new MealPlan
        {
            UserId = dto.UserId,
            Date = DateTime.Now,
            Items = selectedItems
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
        var plan = await _context.MealPlans.FindAsync(id);
        if (plan == null)
            return false;

        _context.MealPlans.Remove(plan);
        await _context.SaveChangesAsync();
        return true;
    }
}