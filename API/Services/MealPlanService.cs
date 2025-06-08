using API.Data;
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
        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null)
            throw new Exception("Kullanıcı bulunamadı");

        var goal = await _context.Goals.FirstOrDefaultAsync(g => g.UserId == dto.UserId);
        if (goal == null)
            throw new Exception("Kullanıcının hedef bilgisi bulunamadı");

        // BMR hesapla (Harris-Benedict)
        double bmr = user.Gender.ToLower() == "male"
            ? 10 * user.Weight + 6.25 * user.Height - 5 * user.Age + 5
            : 10 * user.Weight + 6.25 * user.Height - 5 * user.Age - 161;

        double maintenanceCalories = bmr * 1.5; // Ortalama aktivite katsayısı

        double calorieDiff = (user.Weight - goal.TargetWeight) * 7700 / goal.TargetDays;

        double targetCalories = maintenanceCalories - calorieDiff;

        // Kullanıcıya uygun kaloriyi karşılayacak yiyecekleri çekiyoruz
        // Burada basitçe kaloriye en yakın 5 yiyecek seçelim (sen değiştirebilirsin)
        var foods = await _context.Foods.ToListAsync();

        var selectedFoods = foods
            .OrderBy(f => Math.Abs(f.Calories - (targetCalories / 3))) // Günlük öğün başı kalori hedefi varsayıyorum 3 öğün
            .Take(5)
            .ToList();

        var mealPlan = new MealPlan
        {
            UserId = user.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TargetCalories = targetCalories,
            MealPlanItems = selectedFoods.Select(f => new MealPlanItem
            {
                FoodId = f.Id,
                Quantity = 1
            }).ToList()
        };

        _context.MealPlans.Add(mealPlan);
        await _context.SaveChangesAsync();

        return mealPlan;
    }

    public async Task<MealPlan?> GetMealPlanByUserIdAsync(int userId)
    {
        return await _context.MealPlans
            .Include(mp => mp.MealPlanItems)
            .ThenInclude(mpi => mpi.Food)
            .FirstOrDefaultAsync(mp => mp.UserId == userId);
    }

    public async Task<bool> DeleteMealPlanAsync(int id)
    {
        var mealPlan = await _context.MealPlans.FindAsync(id);
        if (mealPlan == null)
            return false;

        _context.MealPlans.Remove(mealPlan);
        await _context.SaveChangesAsync();

        return true;
    }
}