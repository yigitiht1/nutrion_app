using API.Data;
using Microsoft.EntityFrameworkCore;

public class MealPlanService : IMealPlanService
{
    private readonly AppDbContext _context;

    public MealPlanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MealPlan> CreateMealPlanForUserAsync(int userId)
    {
        // Kullanıcı profilini al
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
        if (profile == null)
            throw new Exception("User profile not found");

        // Kalori hedefini hesapla (UserProfile içindeki methodu kullanalım)
        double targetCalories = profile.CalculateRecommendedCalories();

        // Yeni MealPlan oluştur
        var mealPlan = new MealPlan
        {
            UserId = userId,
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(profile.TargetDays > 0 ? profile.TargetDays : 7),
            TargetCalories = targetCalories,
            MealPlanItems = new List<MealPlanItem>()
        };

        // Kaloriye uygun yiyecekleri seç (örnek: kalori hedefini 3 öğüne böl)
        var foods = await _context.Foods.ToListAsync();
        if (foods.Count == 0)
            throw new Exception("No foods found in database");

        double caloriesPerMeal = targetCalories / 3;

        // Basit örnek: kaloriye en yakın 3 yiyecek seç (kahvaltı, öğle, akşam için)
        var selectedFoods = foods
            .OrderBy(f => Math.Abs(f.Calories - caloriesPerMeal))
            .Take(3)
            .ToList();

        // Her yiyecek için MealPlanItem oluştur, miktarı 1 (1 porsiyon) olarak varsayalım
        foreach (var food in selectedFoods)
        {
            mealPlan.MealPlanItems.Add(new MealPlanItem
            {
                FoodId = food.Id,
                Quantity = 1,
                Food = food
            });
        }

        // MealPlan'ı veritabanına ekle ve kaydet
        _context.MealPlans.Add(mealPlan);
        await _context.SaveChangesAsync();

        return mealPlan;
    }
}