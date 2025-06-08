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
       // Yemek planı oluşturulurken önce kullanıcı profilini buluyoruz
var profile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
if(profile == null)
    throw new Exception("User profile not found");

// TargetCalories hesaplanıyor
double targetCalories = profile.CalculateRecommendedCalories();

// Yeni meal plan nesnesi oluşturuluyor
var mealPlan = new MealPlan
{
    UserId = userId,
    StartDate = DateTime.UtcNow,
    EndDate = DateTime.UtcNow.AddDays(7),
    TargetCalories = targetCalories,
    MealPlanItems = new List<MealPlanItem>()
};

// Foods listesinden örnek seçim
var foods = await _context.Foods.Take(3).ToListAsync();
foreach(var food in foods)
{
    mealPlan.MealPlanItems.Add(new MealPlanItem
    {
        FoodId = food.Id,
        Quantity = 1
    });
}

_context.MealPlans.Add(mealPlan);

try
{
    await _context.SaveChangesAsync();
}
catch(Exception ex)
{
    // Burada inner exception mesajını loglamak çok önemli
    throw new Exception("DB save error: " + ex.InnerException?.Message, ex);
}

return mealPlan;
    }
}