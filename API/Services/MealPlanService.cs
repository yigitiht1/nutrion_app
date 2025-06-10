using API.Data;
using API.Models;
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

   public async Task<MealPlanDto> GenerateWeeklyMealPlanAsync(int userId)
{
    var user = await _context.Users
        .Include(u => u.UserProfile)
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null || user.UserProfile == null)
        throw new Exception("Kullanıcı veya kullanıcı profili bulunamadı.");

    // Burada kalori ihtiyacını hesapla:
    double dailyCalories = user.UserProfile.CalculateRecommendedCalories();

    var mealDistributions = new Dictionary<MealType, double>
    {
        { MealType.Breakfast, 0.3 },
        { MealType.Lunch, 0.35 },
        { MealType.Dinner, 0.25 },
        { MealType.Snack, 0.1 }
    };

    var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
    var plannedMealDtos = new List<PlannedMealDto>();
    var plannedMeals = new List<PlannedMeal>();

    foreach (var day in daysOfWeek)
    {
        foreach (var meal in mealDistributions)
        {
            double targetCals = dailyCalories * meal.Value;

            var foods = await _context.FoodMealTypes
                .Where(fm => fm.MealType == meal.Key)
                .Select(fm => fm.Food)
                .ToListAsync();

            var selectedFoods = new List<Food>();
            double total = 0;

            foreach (var food in foods.OrderBy(f => Guid.NewGuid()))
            {
                if (total + food.Calories <= targetCals)
                {
                    selectedFoods.Add(food);
                    total += food.Calories;
                }

                if (total >= targetCals * 0.95)
                    break;
            }

            plannedMeals.AddRange(selectedFoods.Select(f => new PlannedMeal
            {
                Day = day.ToString(),
                MealType = meal.Key,
                FoodId = f.Id
            }));

            plannedMealDtos.Add(new PlannedMealDto
            {
                Day = day.ToString(),
                MealType = meal.Key,
                Foods = selectedFoods.Select(f => new FoodDto
                {
                    Name = f.Name,
                    Calories = f.Calories,
                    Carbs = f.Carbs,
                    Protein = f.Protein,
                    Fat = f.Fat
                }).ToList()
            });
        }
    }

    var mealPlan = new MealPlan
    {
        UserId = userId,
        StartDate = DateTime.Today,
        EndDate = DateTime.Today.AddDays(6),
        PlannedMeals = plannedMeals
    };

    _context.MealPlans.Add(mealPlan);
    await _context.SaveChangesAsync();

    return new MealPlanDto
    {
        StartDate = mealPlan.StartDate,
        EndDate = mealPlan.EndDate,
        PlannedMeals = plannedMealDtos
    };
}

}