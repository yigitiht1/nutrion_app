namespace API.DTOs
{
    public class CalorieGoalResultDto
    {
        public double DailyCalorieDifference { get; set; }
        public bool IsDeficit { get; set; } // true ise kalori açığı, false ise fazla
        public string Recommendation { get; set; } = string.Empty;
    }
}