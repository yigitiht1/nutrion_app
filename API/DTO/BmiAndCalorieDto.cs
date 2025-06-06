namespace API.DTOs
{
    public class BmiAndCalorieDto
    {
        public double BMI { get; set; }
        public string BMICategory { get; set; } = string.Empty;
        public int RecommendedCalories { get; set; }
        public int TotalCaloriesToday { get; set; }
        public bool IsCalorieLimitExceeded { get; set; }
        public string AdviceMessage { get; set; } = string.Empty;
    }
}