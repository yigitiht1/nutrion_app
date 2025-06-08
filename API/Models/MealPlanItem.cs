using API.Models;

public class MealPlanItem
{
    public int Id { get; set; }
    public int MealPlanId { get; set; }
    public MealPlan MealPlan { get; set; }   // navigation property

    public int FoodId { get; set; }
    public Food Food { get; set; }
    public int Quantity { get; set; }
}