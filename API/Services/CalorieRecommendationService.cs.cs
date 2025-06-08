using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;
public class CalorieRecommendationService : ICalorieRecommendationService
{
    private readonly AppDbContext _context;

    public CalorieRecommendationService(AppDbContext context)
    {
        _context = context;
    }

    // Senkron versiyon: totalCaloriesToday'yi servis içinde hesaplar
public RecommendationDto? GetRecommendationForUser(int userId)
{
    var profile = _context.UserProfiles
        .AsNoTracking()
        .Include(p => p.User)
        .FirstOrDefault(p => p.UserId == userId);

    if (profile == null) return null;

    double bmi = profile.CalculateBMI();
    double goalCalories = profile.CalculateRecommendedCalories();

    var today = DateTime.Today;
    var tomorrow = today.AddDays(1);

    var todayMeals = _context.Meals
        .AsNoTracking()
        .Where(m => m.UserId == userId && m.Date >= today && m.Date < tomorrow)
        .Include(m => m.MealItems)
            .ThenInclude(mi => mi.Food)
        .ToList();

    int totalCaloriesToday = todayMeals
        .SelectMany(m => m.MealItems)
        .Sum(mi => (int)Math.Round(mi.Food.Calories * mi.Quantity)); // Güncellendi!

    int difference = (int)Math.Round(goalCalories - totalCaloriesToday);

    List<Food> suggestedFoods;

            if (difference > 0)
        {
            suggestedFoods = _context.Foods
                .AsNoTracking()
                .Where(f => f.Calories <= difference)  // Kalori açığından yüksek yiyecekler çıkarıldı
                .OrderByDescending(f => f.Protein)
                .Take(3)
                .ToList();
        }
        else
        {
            suggestedFoods = _context.Foods
                .AsNoTracking()
                .Where(f => f.Calories <= 150)
                .OrderByDescending(f => f.Protein)
                .Take(3)
                .ToList();
        }

    return new RecommendationDto
    {
        CalorieDifference = Math.Abs(difference),
        RecommendationType = difference > 0 ? "Deficit" : "Surplus",
        TotalProtein = suggestedFoods.Sum(f => f.Protein),
        TotalCarbs = suggestedFoods.Sum(f => f.Carbs),
        TotalFat = suggestedFoods.Sum(f => f.Fat),
        RecommendedFoods = suggestedFoods.Select(f => new RecommendedFoodDto
        {
            Name = f.Name,
            Calories = f.Calories,
            Protein = f.Protein,
            Carbs = f.Carbs,
            Fat = f.Fat
        }).ToList()
    };
}

    // Async versiyon: totalCaloriesToday parametre olarak alınıyor
    public async Task<RecommendationDto?> GetRecommendationForUserAsync(int userId, int totalCaloriesToday)
    {
        var profile = await _context.UserProfiles
            .AsNoTracking()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null) return null;

        double bmi = profile.CalculateBMI();
        double goalCalories = profile.CalculateRecommendedCalories();

        int difference = (int)Math.Round(goalCalories - totalCaloriesToday);

        List<Food> suggestedFoods;

        if (difference > 0)
        {
            suggestedFoods = await _context.Foods
                .AsNoTracking()
                .Where(f => f.Calories >= 300)
                .OrderByDescending(f => f.Protein)
                .Take(3)
                .ToListAsync();
        }
        else
        {
            suggestedFoods = await _context.Foods
                .AsNoTracking()
                .Where(f => f.Calories <= 150)
                .OrderByDescending(f => f.Protein)
                .Take(3)
                .ToListAsync();
        }

        return new RecommendationDto
        {
            CalorieDifference = Math.Abs(difference),
            RecommendationType = difference > 0 ? "Deficit" : "Surplus",
            TotalProtein = suggestedFoods.Sum(f => f.Protein),
            TotalCarbs = suggestedFoods.Sum(f => f.Carbs),
            TotalFat = suggestedFoods.Sum(f => f.Fat),
            RecommendedFoods = suggestedFoods.Select(f => new RecommendedFoodDto
            {
                Name = f.Name,
                Calories = f.Calories,
                Protein = f.Protein,
                Carbs = f.Carbs,
                Fat = f.Fat
            }).ToList()
        };
    }

    public Task<double> GetRecommendedCaloriesAsync(int userId)
    {
        throw new NotImplementedException();
    }
}