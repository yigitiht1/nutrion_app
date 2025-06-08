public class RecommendationDto
{
    public int CalorieDifference { get; set; }
    public string RecommendationType { get; set; } = string.Empty;
    public double TotalProtein { get; set; }
    public double TotalCarbs { get; set; }
    public double TotalFat { get; set; }
    public List<RecommendedFoodDto> RecommendedFoods { get; set; } = new();
    public List<ActivityDto> RecommendedActivities { get; set; } = new();  // Mevcut, korundu
}

public class RecommendedFoodDto
{
    public string Name { get; set; } = string.Empty;
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
}

public class ActivityDto
{
    public string Name { get; set; } = string.Empty;
    public int CaloriesBurned { get; set; }
}