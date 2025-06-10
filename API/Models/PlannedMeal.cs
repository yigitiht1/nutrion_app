using API.Models;

public class PlannedMeal
{
    public int Id { get; set; }
    public int MealPlanId { get; set; }
    public DateTime Day { get; set; }  // Burayı DateTime yap
    public MealType MealType { get; set; }
    public int FoodId { get; set; }
    public MealPlan MealPlan { get; set; }
    public Food Food { get; set; }
    public int PortionGrams { get; set; }
}