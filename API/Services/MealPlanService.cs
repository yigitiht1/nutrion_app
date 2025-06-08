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

    public async Task<List<Meal>> CreateMealPlanAsync(MealPlanDto dto)
    {
        try
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (userProfile == null)
                throw new Exception("Kullanıcı profili bulunamadı.");

            double dailyCalories = userProfile.CalculateRecommendedCalories();
            var allFoods = await _context.Foods.ToListAsync();

            if (!allFoods.Any())
                throw new Exception("Veritabanında hiç yiyecek bulunamadı.");

            var meals = new List<Meal>();
            var mealTypes = new[] { "Kahvaltı", "Öğle", "Akşam" };

            for (int day = 0; day < dto.DurationDays; day++)
            {
                DateTime mealDate = dto.StartDate.AddDays(day);
                double dailyRemaining = dailyCalories;

                foreach (var type in mealTypes)
                {
                    var meal = new Meal
                    {
                        UserId = dto.UserId,
                        Date = mealDate,
                        MealType = type,
                        MealItems = new List<MealItem>()
                    };

                    while (dailyRemaining > 100)
                    {
                        var food = allFoods
                            .OrderBy(f => Guid.NewGuid())
                            .FirstOrDefault(f => f.Calories < dailyRemaining);

                        if (food == null) break;

                        int quantity = 1;
                        double totalCalories = food.Calories * quantity;

                        if (totalCalories > dailyRemaining) break;

                        meal.MealItems.Add(new MealItem
                        {
                            FoodId = food.Id,
                            Quantity = quantity
                        });

                        dailyRemaining -= totalCalories;
                    }

                    meals.Add(meal);
                }
            }

            await _context.Meals.AddRangeAsync(meals);
            await _context.SaveChangesAsync();

            return meals;
        }
        catch (Exception ex)
        {
            Console.WriteLine("MealPlan oluşturulurken hata oluştu: " + ex.Message);
            throw;
        }
    }
}