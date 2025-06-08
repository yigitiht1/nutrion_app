namespace API.DTOs
{

    public class ActivityRecommendationDto
    {
        public int CalorieDifference { get; set; }
        public string RecommendationType { get; set; } = string.Empty; // "Deficit" veya "Surplus"
        public List<ActivityDto> RecommendedActivities { get; set; } = new List<ActivityDto>();
    }
}