

using System.Collections.Generic;

namespace API.DTOs
{
    public class CalorieGoalResultDto
    {
        public double DailyCalorieDifference { get; set; }
        public bool IsDeficit { get; set; }
        public string Recommendation { get; set; } = string.Empty;
        public List<ExerciseRecommendationDto> ExerciseRecommendations { get; set; } = new();
    }
}