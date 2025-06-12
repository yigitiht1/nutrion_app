using API.Data;
using API.DTOs;
using API.Entities;
using API.Models;
using Microsoft.EntityFrameworkCore;

public class CalorieService : ICalorieService
{
    private readonly AppDbContext _context;

    public CalorieService(AppDbContext context)
    {
        _context = context;
    }

public async Task<double> CalculateCalorieGoalAsync(GoalDto goalDto)
{
    // 1. Kullanıcı profilini getir
    var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == goalDto.UserId);
    if (profile == null)
        throw new ArgumentException("Kullanıcı profili bulunamadı.");

    // 2. Gelen hedef gün sayısı geçerli mi kontrol et
    if (goalDto.TargetDays <= 0)
        throw new ArgumentException("Hedef gün sayısı 0 veya negatif olamaz.");

    // 3. Kullanıcı profilini güncelle
    profile.TargetWeight = goalDto.TargetWeight;
    profile.TargetDays = goalDto.TargetDays;
    await _context.SaveChangesAsync();

    // 4. Goal tablosunu güncelle veya oluştur
    var existingGoal = await _context.Goals.FirstOrDefaultAsync(g => g.UserId == goalDto.UserId);
    if (existingGoal == null)
    {
        existingGoal = new Goal
        {
            UserId = goalDto.UserId,
            TargetWeight = goalDto.TargetWeight,
            TargetDays = goalDto.TargetDays,
            StartDate = goalDto.StartDate.Date
        };
        _context.Goals.Add(existingGoal);
    }
    else
    {
        existingGoal.TargetWeight = goalDto.TargetWeight;
        existingGoal.TargetDays = goalDto.TargetDays;
        existingGoal.StartDate = goalDto.StartDate.Date;
    }
    await _context.SaveChangesAsync();

    // 5. Kalan gün sayısını hesapla (pozitif olmalı)
    var today = DateTime.UtcNow.Date;
    var endDate = existingGoal.StartDate.AddDays(existingGoal.TargetDays);

    int remainingDays = (endDate - today).Days;

    if (remainingDays <= 0)
        throw new InvalidOperationException("Hedef süresi dolmuş veya geçersiz.");

    // 6. Kalori hesabı
    double totalCaloriesToChange = (profile.Weight - goalDto.TargetWeight) * 7700;
    double dailyCalorieChange = totalCaloriesToChange / remainingDays;

    // BMR hesapla (Mifflin-St Jeor Formülü)
    double bmr = 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age +
                 (profile.Gender.ToLower() == "male" ? 5 : -161);

    // Aktivite faktörü olarak 1.2 (çok az aktif) sabit kullanıldı. İstersen burayı genişletebilirsin.
    double dailyCalorieNeed = bmr * 1.2 - dailyCalorieChange;

    return dailyCalorieNeed;
}

    public async Task<BmiAndCalorieDto> CalculateBmiAndCalorieAsync(int userId, int totalCaloriesToday)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
            throw new ArgumentException("Kullanıcı profili bulunamadı.");

        double bmi = profile.CalculateBMI();
        string category = profile.GetBMICategory(bmi);
        double recommendedCalories = profile.CalculateRecommendedCalories();

        bool exceeded = totalCaloriesToday > recommendedCalories;

        string message = exceeded
            ? "Bugünkü kalori hedefinizi aştınız. Daha hafif öğünler tercih edin."
            : "Kalori hedefiniz içinde kaldınız. Dengeli beslenmeye devam edin.";

        return new BmiAndCalorieDto
        {
            BMI = Math.Round(bmi, 2),
            BMICategory = category,
            RecommendedCalories = (int)Math.Round(recommendedCalories),
            TotalCaloriesToday = totalCaloriesToday,
            IsCalorieLimitExceeded = exceeded,
            AdviceMessage = message
        };
    }
public async Task<MealPlanDto> CreatePersonalizedMealPlanAsync(int userId, double dailyCalorieTarget)
{
    var user = await _context.Users.FindAsync(userId);
    if (user == null)
        throw new Exception("Kullanıcı bulunamadı.");

    var mealDistributions = new Dictionary<MealType, double>
    {
        { MealType.Breakfast, 0.3 },
        { MealType.Lunch, 0.35 },
        { MealType.Dinner, 0.25 },
        { MealType.Snack, 0.1 }
    };

    var plannedMeals = new List<PlannedMealDto>();

    foreach (var meal in mealDistributions)
    {
        var targetCalories = dailyCalorieTarget * meal.Value;

        var foods = await _context.FoodMealTypes
            .Where(fm => fm.MealType == meal.Key)
            .Select(fm => fm.Food)
            .Distinct()
            .ToListAsync();

        var selectedFoods = new List<Food>();
        double totalCals = 0, protein = 0, carbs = 0, fat = 0;

        foreach (var food in foods.OrderBy(f => Guid.NewGuid()))
        {
            if (totalCals + food.Calories <= targetCalories)
            {
                selectedFoods.Add(food);
                totalCals += food.Calories;
                protein += food.Protein;
                carbs += food.Carbs;
                fat += food.Fat;
            }

            if (totalCals >= targetCalories * 0.95)
                break;
        }

        plannedMeals.Add(new PlannedMealDto
        {
            Day = DateTime.Now, 
            MealType = meal.Key,
            Foods = selectedFoods.Select(f => new FoodDto
            {
                Name = f.Name,
                Calories = f.Calories,
                Protein = f.Protein,
                Carbs = f.Carbs,
                Fat = f.Fat,
                MealTypes = f.FoodMealTypes.Select(mt => mt.MealType).ToList()
            }).ToList()
        });
    }

    return new MealPlanDto
    {
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddDays(7),
        PlannedMeals = plannedMeals
    };
}
    Task<List<MealDto>> ICalorieService.CreatePersonalizedMealPlanAsync(int userId, double dailyCalorieTarget)
    {
        throw new NotImplementedException();
    }
}