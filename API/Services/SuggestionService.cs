using API.Models;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class SuggestionService : ISuggestionService
    {
        private readonly IUserProfile _userProfileService;
        private readonly IActivityService _activityService;

        public SuggestionService(IUserProfile userProfileService, IActivityService activityService)
        {
            _userProfileService = userProfileService;
            _activityService = activityService;
        }

        public async Task<string> GetSuggestionAsync(int userId)
        {
            var profile = await _userProfileService.GetUserProfileByUserIdAsync(userId);
            if (profile == null)
                return "Kullanıcı profili bulunamadı.";

            var activities = await _activityService.GetActivitiesByUserIdAsync(userId);
            double caloriesBurned = activities.Sum(a => a.CaloriesBurned);
            double recommendedCalories = profile.CalculateRecommendedCalories();
            double calorieDifference = recommendedCalories - caloriesBurned;

            double bmi = profile.CalculateBMI();
            string bmiCategory = profile.GetBMICategory(bmi);

            if (calorieDifference > 500)
                return $"Kilo vermek için kalori açığınız yeterince yüksek değil. Daha fazla egzersiz yapabilirsiniz. (BMI: {bmiCategory})";
            else if (calorieDifference < -500)
                return $"Kilo almak için kalori fazlanız yüksek, beslenmenizi takip edin. (BMI: {bmiCategory})";
            else
                return $"Kalori hedefinize uygunsunuz, mevcut rutininize devam edin. (BMI: {bmiCategory})";
        }
    }
}