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
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] GetRecommendationForUser: {ex.Message}");
            return null;
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

            if (profile == null) return null;

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
                TotalProtein = 0, // async versiyon için detaylı makro hesaplanmıyor
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
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] GetRecommendationForUserAsync: {ex.Message}");
            return null;
        }
    }

    public Task<double> GetRecommendedCaloriesAsync(int userId)
    {
        throw new NotImplementedException();
    }
}