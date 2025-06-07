using API.Models;

namespace API.DTOs
{
    public class CalorieAdviceDto
    {
        public double RemainingCalories { get; set; }
        public List<Food> RecommendedFoods { get; set; } = new();
        public List<Activity> RecommendedActivities { get; set; } = new();
        public string WeightGoalProgress { get; set; } = string.Empty;
    }
}