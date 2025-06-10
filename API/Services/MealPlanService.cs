using API.Data;
using Microsoft.EntityFrameworkCore;

public class MealPlanService : IMealPlanService
{
    private readonly AppDbContext _context;

    public MealPlanService(AppDbContext context)
    {
        _context = context;
    }

public async Task CreateMealPlanAsync(int userId, MealPlanDto dto)
{
    var mealPlan = new MealPlan
    {
        UserId = userId,
        StartDate = dto.StartDate ?? DateTime.Now,
        EndDate = dto.EndDate ?? DateTime.Now,
        PlannedMeals = new List<PlannedMeal>()
    };

    foreach (var plannedMealDto in dto.PlannedMeals)
    {
        if (plannedMealDto.Foods != null && plannedMealDto.Foods.Any())
        {
            foreach (var foodDto in plannedMealDto.Foods)
            {
                var food = await _context.Foods.FirstOrDefaultAsync(f => f.Name == foodDto.Name);
                if (food == null)
                    throw new Exception($"Food '{foodDto.Name}' bulunamadı.");

                mealPlan.PlannedMeals.Add(new PlannedMeal
                {
                    Day = plannedMealDto.Day,
                    MealType = plannedMealDto.MealType,
                    FoodId = food.Id
                });
            }
        }
        else
        {
            throw new Exception("PlannedMealDto içinde Food listesi boş olamaz.");
        }
    }

    _context.MealPlans.Add(mealPlan);
    await _context.SaveChangesAsync();
}

private async Task<int> GetFoodIdByNameAsync(string name)
{
    var food = await _context.Foods.FirstOrDefaultAsync(f => f.Name == name);
    if (food == null)
        throw new Exception($"Food with name '{name}' not found.");
    return food.Id;
}

    public async Task<List<MealPlanDto>> GetMealPlansByUserAsync(int userId)
{
    var plans = await _context.MealPlans
        .Include(p => p.PlannedMeals)
        .ThenInclude(pm => pm.Food)  // Food bilgisi dahil et
        .Where(p => p.UserId == userId)
        .ToListAsync();

    var result = new List<MealPlanDto>();

    foreach (var plan in plans)
    {
        // Aynı gün ve aynı öğün için PlannedMeal'ları gruplandır
        var groupedPlannedMeals = plan.PlannedMeals
            .GroupBy(pm => new { pm.Day, pm.MealType })
            .Select(g => new PlannedMealDto
            {
                Day = g.Key.Day,
                MealType = g.Key.MealType,
                Foods = g.Select(pm => new FoodDto
                {
                    Name = pm.Food.Name,
                    Calories = pm.Food.Calories,
                    Protein = pm.Food.Protein,
                    Carbs = pm.Food.Carbs,
                    Fat = pm.Food.Fat,
                    MealTypes = pm.Food.FoodMealTypes.Select(mt => mt.MealType).ToList()
                }).ToList()
            }).ToList();

        result.Add(new MealPlanDto
        {
            StartDate = plan.StartDate,
            EndDate = plan.EndDate,
            PlannedMeals = groupedPlannedMeals
        });
    }

    return result;
}
    public async Task DeleteMealPlanAsync(int userId, int mealPlanId)
    {
        var plan = await _context.MealPlans
            .Include(p => p.PlannedMeals)
            .FirstOrDefaultAsync(p => p.Id == mealPlanId && p.UserId == userId);

        if (plan != null)
        {
            _context.PlannedMeals.RemoveRange(plan.PlannedMeals);
            _context.MealPlans.Remove(plan);
            await _context.SaveChangesAsync();
        }
    }
}