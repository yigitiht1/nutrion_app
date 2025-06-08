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

    // Senkron versiyon: totalCaloriesToday'yi servis içinde hesaplar ve makroları da döner
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
            .Sum(mi => (int)Math.Round(mi.Food.Calories * mi.Quantity));

        double totalProteinToday = todayMeals
            .SelectMany(m => m.MealItems)
            .Sum(mi => mi.Food.Protein * mi.Quantity);

        double totalCarbsToday = todayMeals
            .SelectMany(m => m.MealItems)
            .Sum(mi => mi.Food.Carbs * mi.Quantity);

        double totalFatToday = todayMeals
            .SelectMany(m => m.MealItems)
            .Sum(mi => mi.Food.Fat * mi.Quantity);

        int difference = (int)Math.Round(goalCalories - totalCaloriesToday);

        List<Food> suggestedFoods;

        if (difference > 0)
        {
            suggestedFoods = _context.Foods
                .AsNoTracking()
                .Where(f => f.Calories <= difference)
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

            // Kullanıcının bugün yediği toplam makrolar
            TotalProtein = totalProteinToday,
            TotalCarbs = totalCarbsToday,
            TotalFat = totalFatToday,

            // Önerilen yiyecekler
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

    // Async versiyon: totalCaloriesToday parametre olarak alınıyor, ama istersen bunu da servis içinde hesaplatabilirsin
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
                .Where(f => f.Calories <= difference)
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

            // Burada toplam makroları async method'da parametre olarak almadığımız için sıfır gönderdim.
            // İstersen asenkron versiyonu da kullanıcı yemeklerini hesaplayacak şekilde güncelleyebiliriz.
            TotalProtein = 0,
            TotalCarbs = 0,
            TotalFat = 0,

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