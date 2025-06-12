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

    // SENKRON versiyon (mevcut hesaplamaları koruyor)
    public RecommendationDto? GetRecommendationForUser(int userId)
    {
        try
        {
            var profile = _context.UserProfiles
                .AsNoTracking()
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userId);

            if (profile == null)
                return new RecommendationDto { AlertMessage = "Kullanıcı profili bulunamadı." };

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

            var mealItems = todayMeals.SelectMany(m => m.MealItems).Where(mi => mi.Food != null).ToList();

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
                var recommendedActivities = GetDefaultActivities();

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
            return new RecommendationDto { AlertMessage = "Bir hata oluştu, lütfen tekrar deneyin." };
        }
    }

    // ASENKRON versiyon (hedef kilo ve süreyi hesaba katar)
    public async Task<RecommendationDto> GetRecommendationForUserAsync(int userId, int day)
    {
        var profile = await _context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            throw new Exception("Kullanıcı profili bulunamadı.");

        var goal = await _context.Goals
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.UserId == userId);

        double recommendedCalories;
        string recommendationType = "maintenance";
        string alertMessage = "";
        int calorieDifference = 0;

        if (goal != null && goal.TargetDays >= 7 && goal.TargetWeight > 0)
        {
            int daysPassed = (DateTime.UtcNow.Date - goal.StartDate.Date).Days;
            int remainingDays = Math.Max(1, goal.TargetDays - daysPassed);
            double weightDiff = profile.Weight - goal.TargetWeight;
            double totalCalorieChange = weightDiff * 7700;
            double dailyCalorieChange = totalCalorieChange / remainingDays;

            double bmr = 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age +
                         (profile.Gender.ToLower() == "male" ? 5 : -161);

            recommendedCalories = bmr * 1.2 - dailyCalorieChange;
            calorieDifference = (int)Math.Round(dailyCalorieChange);

            if (weightDiff > 0)
            {
                recommendationType = "deficit";
                alertMessage = $"Hedef kiloya ulaşmak için günde yaklaşık {calorieDifference} kalori açık oluşturmanız gerekiyor.";
            }
            else if (weightDiff < 0)
            {
                recommendationType = "surplus";
                alertMessage = $"Hedef kiloya ulaşmak için günde yaklaşık {Math.Abs(calorieDifference)} kalori fazlası almalısınız.";
            }
            else
            {
                alertMessage = "Hedef kilonuz mevcut kilonuzla aynı.";
            }
        }
        else
        {
            double bmr = 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age +
                         (profile.Gender.ToLower() == "male" ? 5 : -161);
            recommendedCalories = bmr * 1.2;
            alertMessage = "Hedef bilgisi bulunamadı. Günlük öneri mevcut kilonuza göre yapıldı.";
        }

        var today = DateTime.UtcNow.Date;

        var todayMeals = await _context.Meals
            .Where(m => m.UserId == userId && m.Date.Date == today)
            .Include(m => m.MealItems)
                .ThenInclude(mi => mi.Food)
            .ToListAsync();

        var mealItems = todayMeals.SelectMany(m => m.MealItems)
            .Where(mi => mi.Food != null)
            .ToList();

        double totalCaloriesToday = mealItems.Sum(mi => mi.Food.Calories * mi.Quantity);
        double totalProteinToday = mealItems.Sum(mi => mi.Food.Protein * mi.Quantity);
        double totalCarbsToday = mealItems.Sum(mi => mi.Food.Carbs * mi.Quantity);
        double totalFatToday = mealItems.Sum(mi => mi.Food.Fat * mi.Quantity);

        int calorieDiff = (int)Math.Round(totalCaloriesToday - recommendedCalories);

        var foodRecommendations = new List<RecommendedFoodDto>();
        var activityRecommendations = new List<ActivityDto>();

        if (calorieDiff < 0 && Math.Abs(calorieDiff) > 200)
        {
            foodRecommendations = _context.Foods
                .AsNoTracking()
                .Where(f => f.Calories <= Math.Abs(calorieDiff))
                .OrderByDescending(f => f.Protein)
                .Take(3)
                .Select(f => new RecommendedFoodDto
                {
                    Name = f.Name,
                    Calories = f.Calories,
                    Protein = f.Protein,
                    Carbs = f.Carbs,
                    Fat = f.Fat
                })
                .ToList();

            alertMessage = "Kalori açığınız var, önerilen yiyecekleri tüketebilirsiniz.";
        }
        else if (calorieDiff > 0)
        {
            activityRecommendations = GetDefaultActivities();

            alertMessage = calorieDiff > 500
                ? "Kalori fazlalığınız yüksek, önerilen aktiviteleri yaparak dengeleyin."
                : "Bir miktar kalori fazlanız var, hareket etmeye devam edin.";
        }

        return new RecommendationDto
        {
            CalorieDifference = calorieDiff,
            RecommendationType = recommendationType,
            TotalProtein = totalProteinToday,
            TotalCarbs = totalCarbsToday,
            TotalFat = totalFatToday,
            RecommendedFoods = foodRecommendations,
            RecommendedActivities = activityRecommendations,
            AlertMessage = alertMessage
        };
    }

    private List<ActivityDto> GetDefaultActivities()
    {
        return new List<ActivityDto>
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
}