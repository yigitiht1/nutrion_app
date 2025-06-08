using API.Data;
using API.DTOs;
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
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == goalDto.UserId);
        if (profile == null)
            throw new ArgumentException("Kullanıcı profili bulunamadı.");

        profile.TargetWeight = goalDto.TargetWeight;
        profile.TargetDays = goalDto.TargetDays;

        await _context.SaveChangesAsync();

        double totalCaloriesToChange = (profile.Weight - goalDto.TargetWeight) * 7700;
        double dailyCalorieChange = totalCaloriesToChange / goalDto.TargetDays;

        double bmr = 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age + (profile.Gender.ToLower() == "male" ? 5 : -161);

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

    public Task<List<MealDto>> CreatePersonalizedMealPlanAsync(int userId, double dailyCalorieTarget)
    {
        throw new NotImplementedException();
    }
}