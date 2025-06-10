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

    public async Task CreateMealPlanAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new Exception("Kullanıcı bulunamadı.");

        DateTime planStartDate = startDate?.Date ?? DateTime.Now.Date;
        DateTime planEndDate = endDate?.Date ?? DateTime.Now.Date;

        int totalDays = (planEndDate - planStartDate).Days + 1;
        if (totalDays <= 0)
            throw new Exception("Bitiş tarihi, başlangıç tarihinden önce olamaz.");

        int targetCalories = user.DailyCalorieNeed - user.CalorieDeficit;
        if (targetCalories <= 0)
            throw new Exception("Hedef kalori miktarı geçersiz.");

        var foods = await _context.Foods
            .OrderBy(f => f.CaloriesPer100g)
            .ToListAsync();

        if (!foods.Any())
            throw new Exception("Yiyecek verisi bulunamadı.");

        var mealPlan = new MealPlan
        {
            UserId = userId,
            StartDate = planStartDate,
            EndDate = planEndDate,
            PlannedMeals = new List<PlannedMeal>()
        };

        int mealsPerDay = 3; 
        int caloriesPerMeal = targetCalories / mealsPerDay;

        for (int dayIndex = 0; dayIndex < totalDays; dayIndex++)
        {
            DateTime currentDay = planStartDate.AddDays(dayIndex);

            for (int mealTypeIndex = 1; mealTypeIndex <= mealsPerDay; mealTypeIndex++)
            {
                int remainingCalories = caloriesPerMeal;

                foreach (var food in foods)
                {
                    if (remainingCalories <= 0)
                        break;

                    int portionGram = 100;
                    int portionCalories = (food.CaloriesPer100g * portionGram) / 100;

                    if (portionCalories <= remainingCalories)
                    {
                        mealPlan.PlannedMeals.Add(new PlannedMeal
                        {
                            Day = currentDay,  // Burada DateTime olarak bırak
                            MealType = (MealType)mealTypeIndex,
                            FoodId = food.Id,
                            PortionGrams = portionGram
                        });

                        remainingCalories -= portionCalories;
                    }
                }
            }
        }

        _context.MealPlans.Add(mealPlan);
        await _context.SaveChangesAsync();
    }

    public async Task<List<MealPlanDto>> GetMealPlansByUserAsync(int userId)
    {
        var plans = await _context.MealPlans
            .Include(p => p.PlannedMeals)
            .ThenInclude(pm => pm.Food)
            .Where(p => p.UserId == userId)
            .ToListAsync();

        var result = new List<MealPlanDto>();

        foreach (var plan in plans)
        {
            var groupedPlannedMeals = plan.PlannedMeals
                .GroupBy(pm => new { pm.Day, pm.MealType })
                .Select(g => new PlannedMealDto
                {
                    Day = g.Key.Day, // Burada string format kullan
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

        double dailyCalories = user.UserProfile.CalculateRecommendedCalories();

        var mealDistributions = new Dictionary<MealType, double>
        {
            { MealType.Breakfast, 0.3 },
            { MealType.Lunch, 0.35 },
            { MealType.Dinner, 0.25 },
            { MealType.Snack, 0.1 }
        };

        DateTime today = DateTime.Today;
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        DateTime startOfWeek = today.AddDays(-diff);

        var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
        var plannedMealDtos = new List<PlannedMealDto>();
        var plannedMeals = new List<PlannedMeal>();

        foreach (var day in daysOfWeek)
        {
            DateTime currentDate = startOfWeek.AddDays((int)day);

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
                    Day = currentDate,  // Burada da DateTime olarak bırak
                    MealType = meal.Key,
                    FoodId = f.Id
                }));

                plannedMealDtos.Add(new PlannedMealDto
                {
                    Day = currentDate,
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
            StartDate = startOfWeek,
            EndDate = startOfWeek.AddDays(6),
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

    public Task CreateMealPlanAsync(int userId, MealPlanDto dto)
    {
        throw new NotImplementedException();
    }
}