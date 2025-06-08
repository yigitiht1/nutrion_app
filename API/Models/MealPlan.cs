using API.Models;

public class MealPlan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double TargetCalories { get; set; }
    public List<MealPlanItem> MealPlanItems { get; set; } = new();
}