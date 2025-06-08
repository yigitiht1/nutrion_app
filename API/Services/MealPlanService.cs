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
        var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
        if (userProfile == null)
            throw new ArgumentException("Kullanıcı profili bulunamadı.");

        // Kalori hedefini hesapla
        double dailyCalories = userProfile.CalculateRecommendedCalories();

        // Günlük öğün tipleri (kahvaltı, öğle, akşam, ara öğün)
        var mealTypes = new[] { "Breakfast", "Lunch", "Dinner", "Snack" };

        // Veritabanındaki tüm yiyecekleri çek
        var allFoods = await _context.Foods.ToListAsync();

        var meals = new List<Meal>();

        for (int day = 0; day < dto.DurationDays; day++)
        {
            DateTime currentDate = dto.StartDate.Date.AddDays(day);

            foreach (var mealType in mealTypes)
            {
                var meal = new Meal
                {
                    UserId = dto.UserId,
                    Date = currentDate,
                    MealType = mealType
                };

                // Kaloriye göre yaklaşık olarak bu öğünde kaç kalori hedeflenir (ör: toplam kalori 2000 ise %25 breakfast, %30 lunch, %30 dinner, %15 snack)
                double mealCalorieTarget = mealType switch
                {
                    "Breakfast" => dailyCalories * 0.25,
                    "Lunch" => dailyCalories * 0.3,
                    "Dinner" => dailyCalories * 0.3,
                    "Snack" => dailyCalories * 0.15,
                    _ => dailyCalories / 4
                };

                double caloriesAccumulated = 0;
                var mealItems = new List<MealItem>();

                // Basitçe yiyecekleri kaloriye göre seçiyoruz, burada daha iyi algoritma yapılabilir
                foreach (var food in allFoods.OrderBy(f => f.Calories))
                {
                    if (caloriesAccumulated >= mealCalorieTarget)
                        break;

                    // Tahmini kaç porsiyon alacağız (en az 1)
                    int quantity = 1;

                    // Kaloriyi geçmeyecek kadar porsiyon al
                    while (caloriesAccumulated + food.Calories * quantity <= mealCalorieTarget)
                    {
                        quantity++;
                    }
                    quantity = Math.Max(1, quantity - 1);

                    caloriesAccumulated += food.Calories * quantity;

                    mealItems.Add(new MealItem
                    {
                        FoodId = food.Id,
                        Quantity = quantity
                    });
                }

                meal.MealItems = mealItems;
                meals.Add(meal);
            }
        }

        // Önce varsa eski planları silmek iyi olabilir (opsiyonel)

        // Yeni planı kaydet
        await _context.Meals.AddRangeAsync(meals);
        await _context.SaveChangesAsync();

        return meals;
    }

    public async Task<List<Meal>?> GetMealPlanByUserIdAsync(int userId)
    {
        return await _context.Meals
            .Include(m => m.MealItems)
            .ThenInclude(mi => mi.Food)
            .Where(m => m.UserId == userId)
            .ToListAsync();
    }
}