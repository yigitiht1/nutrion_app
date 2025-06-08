using API.Services;

public interface ISuggestionService
{
    Task<string> GetSuggestionAsync(int userId);
}

public class SuggestionService : ISuggestionService
{
    private readonly IUserProfile _userProfile;
    private readonly IActivityService _activityService;

    public SuggestionService(IUserProfile userProfile, IActivityService activityService)
    {
        _userProfile = userProfile;
        _activityService = activityService;
    }

    public async Task<string> GetSuggestionAsync(int userId)
    {
        var profile = await _userProfile.GetUserProfileByUserIdAsync(userId);
        if (profile == null) return "Kullanıcı profili bulunamadı.";

        var today = DateTime.Today;
        var activities = (await _activityService.GetActivitiesByUserIdAsync(userId))
                            .Where(a => a.Date.Date == today)
                            .ToList();

        double caloriesBurned = activities.Sum(a => a.CaloriesBurned);
        double recommendedCalories = profile.CalculateRecommendedCalories();
        double calorieDifference = recommendedCalories - caloriesBurned;

        double bmi = profile.CalculateBMI();
        string bmiCategory = profile.GetBMICategory(bmi);

        if (calorieDifference > 500)
            return $"Kalori açığınız düşük. Daha fazla egzersiz yapabilirsiniz. (BMI: {bmiCategory})";
        else if (calorieDifference < -500)
            return $"Kalori fazlanız fazla. Beslenmenizi gözden geçirin. (BMI: {bmiCategory})";
        else
            return $"Harika! Kalori dengeniz yerinde. (BMI: {bmiCategory})";
    }
}