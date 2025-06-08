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

    // Senkron versiyon (isteğe bağlı)
    public RecommendationDto? GetRecommendationForUser(int userId)
    {
        try
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

            var mealItems = todayMeals
                .SelectMany(m => m.MealItems)
                .Where(mi => mi.Food != null)
                .ToList();

            int totalCaloriesToday = (int)Math.Round(mealItems.Sum(mi => mi.Food.Calories * mi.Quantity));
            double totalProteinToday = mealItems.Sum(mi => mi.Food.Protein * mi.Quantity);
            double totalCarbsToday = mealItems.Sum(mi => mi.Food.Carbs * mi.Quantity);
            double totalFatToday = mealItems.Sum(mi => mi.Food.Fat * mi.Quantity);

            int difference = (int)Math.Round(goalCalories - totalCaloriesToday);

            if (difference > 0) // kalori açığı varsa -> besin önerisi
            {
                var suggestedFoods = _context.Foods
                    .AsNoTracking()
                    .Where(f => f.Calories <= difference)
                    .OrderByDescending(f => f.Protein)
                    .Take(3)
                    .ToList();

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
                    RecommendedActivities = new List<ActivityDto>()
                };
            }
            else // kalori fazlası varsa -> aktivite önerisi
            {
                var recommendedActivities = new List<ActivityDto>
                {
                    new ActivityDto { Name = "20 Dakika Koşu", CaloriesBurned = 250 },
                    new ActivityDto { Name = "10 Dakika Merdiven Çıkma", CaloriesBurned = 120 },
                    new ActivityDto { Name = "30 Dakika Bisiklet", CaloriesBurned = 200 },
                    new ActivityDto { Name = "15 Dakika Yüzme", CaloriesBurned = 180 }
                };

                return new RecommendationDto
                {
                    CalorieDifference = Math.Abs(difference),
                    RecommendationType = "Surplus",
                    TotalProtein = totalProteinToday,
                    TotalCarbs = totalCarbsToday,
                    TotalFat = totalFatToday,
                    RecommendedFoods = new List<RecommendedFoodDto>(), // Fazla kalori için yiyecek önerisi yok
                    RecommendedActivities = recommendedActivities
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] GetRecommendationForUser: {ex.Message}");
            return null;
        }
    }

    // Asenkron versiyon
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
            int difference = (int)Math.Round(goalCalories - totalCaloriesToday);

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

            if (difference > 0) // kalori açığı varsa -> yiyecek önerisi
            {
                suggestedFoods = await _context.Foods
                    .AsNoTracking()
                    .Where(f => f.Calories <= difference)
                    .OrderByDescending(f => f.Protein)
                    .Take(3)
                    .ToListAsync();
            }
            else // kalori fazlası varsa -> aktivite önerisi
            {
                recommendedActivities = new List<ActivityDto>
                {
                    new ActivityDto { Name = "20 Dakika Koşu", CaloriesBurned = 250 },
                    new ActivityDto { Name = "10 Dakika Merdiven Çıkma", CaloriesBurned = 120 },
                    new ActivityDto { Name = "30 Dakika Bisiklet", CaloriesBurned = 200 },
                    new ActivityDto { Name = "15 Dakika Yüzme", CaloriesBurned = 180 }
                };
            }

            return new RecommendationDto
            {
                CalorieDifference = Math.Abs(difference),
                RecommendationType = difference > 0 ? "Deficit" : "Surplus",
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
                RecommendedActivities = recommendedActivities
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] GetRecommendationForUserAsync: {ex.Message}");
            return null;
        }
    }

    // İstersen bu metodu da uygun şekilde implement edebilirsin
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