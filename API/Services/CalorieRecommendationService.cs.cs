using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CalorieRecommendationService : ICalorieRecommendationService
{
    private readonly AppDbContext _context;

    public CalorieRecommendationService(AppDbContext context)
    {
        _context = context;
    }

    public RecommendationDto? GetRecommendationForUser(int userId)
{
    try
    {
        var profile = _context.UserProfiles
            .AsNoTracking()
            .Include(p => p.User)
            .FirstOrDefault(p => p.UserId == userId);

        if (profile == null)
            return new RecommendationDto
            {
                AlertMessage = "Kullanıcı profili bulunamadı."
            };

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

        var mealItems = todayMeals
            .SelectMany(m => m.MealItems)
            .Where(mi => mi.Food != null)
            .ToList();

        int totalCaloriesToday = (int)Math.Round(mealItems.Sum(mi => mi.Food.Calories * mi.Quantity));
        double totalProteinToday = mealItems.Sum(mi => mi.Food.Protein * mi.Quantity);
        double totalCarbsToday = mealItems.Sum(mi => mi.Food.Carbs * mi.Quantity);
        double totalFatToday = mealItems.Sum(mi => mi.Food.Fat * mi.Quantity);

        int difference = (int)Math.Round(totalCaloriesToday - goalCalories);

        if (difference < 0)
        {
            int deficit = Math.Abs(difference);
            var suggestedFoods = _context.Foods
                .AsNoTracking()
                .Where(f => f.Calories <= deficit)
                .OrderByDescending(f => f.Protein)
                .Take(3)
                .ToList();

            string alertMessage = deficit > 500
                ? "Büyük bir kalori açığınız var, beslenmenize dikkat edin."
                : "Kalori açığınız var, önerilen yiyecekleri tüketebilirsiniz.";

            return new RecommendationDto
            {
                CalorieDifference = difference,
                RecommendationType = "Deficit",
                TotalProtein = totalProteinToday,
                TotalCarbs = totalCarbsToday,
                TotalFat = totalFatToday,
                RecommendedFoods = suggestedFoods.Select(f => new RecommendedFoodDto
                {
                    Name = f.Name,
                    Calories = f.Calories,
                    Protein = f.Protein,
                    Carbs = f.Carbs,
                    Fat = f.Fat
                }).ToList(),
                RecommendedActivities = new List<ActivityDto>(),
                AlertMessage = alertMessage
            };
        }
        else
        {
            var recommendedActivities = new List<ActivityDto>
            {
                new ActivityDto { Name = "60 Dakika Koşu", CaloriesBurned = 900 },
                new ActivityDto { Name = "30 Dakika Bisiklet", CaloriesBurned = 270 },
                new ActivityDto { Name = "45 Dakika Merdiven Çıkma", CaloriesBurned = 450 },
                new ActivityDto { Name = "60 Dakika Yüzme", CaloriesBurned = 720 },
                new ActivityDto { Name = "45 Dakika Zumba", CaloriesBurned = 400 },
                new ActivityDto { Name = "30 Dakika İp Atlama", CaloriesBurned = 450 },
                new ActivityDto { Name = "60 Dakika Basketbol", CaloriesBurned = 480 },
                new ActivityDto { Name = "60 Dakika Hızlı Yürüyüş", CaloriesBurned = 360 }
            };

            string alertMessage = difference > 500
                ? "Kalori fazlalığınız yüksek, önerilen aktiviteleri yaparak dengeleyin."
                : "Bir miktar kalori fazlanız var, hareket etmeye devam edin.";

            return new RecommendationDto
            {
                CalorieDifference = difference,
                RecommendationType = "Surplus",
                TotalProtein = totalProteinToday,
                TotalCarbs = totalCarbsToday,
                TotalFat = totalFatToday,
                RecommendedFoods = new List<RecommendedFoodDto>(),
                RecommendedActivities = recommendedActivities,
                AlertMessage = alertMessage
            };
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] GetRecommendationForUser: {ex.Message}");
        return new RecommendationDto
        {
            AlertMessage = "Bir hata oluştu, lütfen tekrar deneyin."
        };
    }
}

    public async Task<RecommendationDto?> GetRecommendationForUserAsync(int userId, int totalCaloriesToday)
    {
        try
        {
            var profile = await _context.UserProfiles
                .AsNoTracking()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return null;

            double goalCalories = profile.CalculateRecommendedCalories();
            int difference = (int)Math.Round(totalCaloriesToday - goalCalories); // Alınan - Hedef

            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var todayMeals = await _context.Meals
                .AsNoTracking()
                .Where(m => m.UserId == userId && m.Date >= today && m.Date < tomorrow)
                .Include(m => m.MealItems)
                    .ThenInclude(mi => mi.Food)
                .ToListAsync();

            var mealItems = todayMeals
                .SelectMany(m => m.MealItems)
                .Where(mi => mi.Food != null)
                .ToList();

            double totalProteinToday = mealItems.Sum(mi => mi.Food.Protein * mi.Quantity);
            double totalCarbsToday = mealItems.Sum(mi => mi.Food.Carbs * mi.Quantity);
            double totalFatToday = mealItems.Sum(mi => mi.Food.Fat * mi.Quantity);

            List<Food> suggestedFoods = new();
            List<ActivityDto> recommendedActivities = new();

            if (difference < 0) // Kalori açığı -> yiyecek önerisi
            {
                int deficit = Math.Abs(difference);
                suggestedFoods = await _context.Foods
                    .AsNoTracking()
                    .Where(f => f.Calories <= deficit)
                    .OrderByDescending(f => f.Protein)
                    .Take(3)
                    .ToListAsync();
            }
            else // Kalori fazlası -> aktivite önerisi
            {
                recommendedActivities = new List<ActivityDto>
                {
                    new ActivityDto { Name = "60 Dakika Koşu", CaloriesBurned = 900 },
                    new ActivityDto { Name = "30 Dakika Bisiklet", CaloriesBurned = 270 },
                    new ActivityDto { Name = "45 Dakika Merdiven Çıkma", CaloriesBurned = 450 },
                    new ActivityDto { Name = "60 Dakika Yüzme", CaloriesBurned = 720 },
                    new ActivityDto { Name = "45 Dakika Zumba", CaloriesBurned = 400 },
                    new ActivityDto { Name = "30 Dakika İp Atlama", CaloriesBurned = 450 },
                    new ActivityDto { Name = "60 Dakika Basketbol", CaloriesBurned = 480 },
                    new ActivityDto { Name = "60 Dakika Hızlı Yürüyüş", CaloriesBurned = 360 }
                };
            }

          return new RecommendationDto
        {
            CalorieDifference = difference, // Pozitif/negatif farkı gösterir
            RecommendationType = difference < 0 ? "Deficit" : "Surplus",
            TotalProtein = totalProteinToday,
            TotalCarbs = totalCarbsToday,
            TotalFat = totalFatToday,
            RecommendedFoods = suggestedFoods.Select(f => new RecommendedFoodDto
            {
                Name = f.Name,
                Calories = f.Calories,
                Protein = f.Protein,
                Carbs = f.Carbs,
                Fat = f.Fat
            }).ToList(),
            RecommendedActivities = recommendedActivities,
            AlertMessage = difference < 0
                ? (Math.Abs(difference) > 500
                    ? "Büyük bir kalori açığınız var, beslenmenize dikkat edin."
                    : "Kalori açığınız var, önerilen yiyecekleri tüketebilirsiniz.")
                : (difference > 500
                    ? "Kalori fazlalığınız yüksek, önerilen aktiviteleri yaparak dengeleyin."
                    : "Bir miktar kalori fazlanız var, hareket etmeye devam edin.")
        };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] GetRecommendationForUserAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<double> GetRecommendedCaloriesAsync(int userId)
    {
        var profile = await _context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            throw new Exception("User profile not found");

        return profile.CalculateRecommendedCalories();
    }
}