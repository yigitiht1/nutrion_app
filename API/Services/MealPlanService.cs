using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

public class MealPlanService
{
    private readonly AppDbContext _context;

    public MealPlanService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Kullanıcının hedef kalori bilgisine göre, öğün türlerine göre uygun yemeklerden
    /// basit bir MealPlan oluşturur.
    /// </summary>
    public async Task<MealPlan> CreateMealPlanAsync(int userId, DateTime startDate, DateTime endDate)
    {
        var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
        if (userProfile == null) throw new Exception("UserProfile bulunamadı.");

        double targetCalories = userProfile.CalculateRecommendedCalories();

        // Belirtilen tarih aralığında her gün için plan oluştur
        var mealPlan = new MealPlan
        {
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
            TargetCalories = targetCalories,
            MealPlanItems = new List<MealPlanItem>()
        };

        // Öğün tipleri (Breakfast, Lunch, Dinner, Snack)
        var mealTypes = Enum.GetValues(typeof(MealType)).Cast<MealType>().ToList();

        // Toplam kaloriyi öğünlere eşit dağıtalım (örneğin 4 öğün varsa %25'er)
        double caloriesPerMeal = targetCalories / mealTypes.Count;

        foreach (var mealType in mealTypes)
        {
            // O öğün için uygun yiyecekleri al
            var foodsForMeal = await _context.FoodMealTypes
                .Include(fmt => fmt.Food)
                .Where(fmt => fmt.MealType == mealType)
                .Select(fmt => fmt.Food)
                .ToListAsync();

            if (!foodsForMeal.Any()) continue;

            double caloriesAdded = 0;

            foreach (var food in foodsForMeal)
            {
                if (caloriesAdded >= caloriesPerMeal)
                    break;

                // Geri kalan kaloriyi hesapla
                double remainingCalories = caloriesPerMeal - caloriesAdded;

                // Yemek başına kalori - 100 gram üzerinden
                double quantityGrams = (remainingCalories / food.Calories) * 100;

                if (quantityGrams < 30) // Minimum porsiyon miktarı 30 gram
                    quantityGrams = 30;

                caloriesAdded += (food.Calories * quantityGrams) / 100;

                mealPlan.MealPlanItems.Add(new MealPlanItem
                {
                    FoodId = food.Id,
                    Quantity = (int)quantityGrams
                });
            }
        }

        _context.MealPlans.Add(mealPlan);
        await _context.SaveChangesAsync();

        return mealPlan;
    }
}