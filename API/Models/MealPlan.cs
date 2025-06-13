using API.Models;

public class MealPlan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<PlannedMeal> PlannedMeals { get; set; } = new();

    public User? User { get; set; } 

    public ICollection<MealPlanItem> MealPlanItems { get; set; } = new List<MealPlanItem>();
}