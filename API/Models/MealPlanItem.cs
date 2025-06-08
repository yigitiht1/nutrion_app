using API.Models;

public class MealPlanItem
{
    public int Id { get; set; }
    public int MealPlanId { get; set; }
    public int FoodId { get; set; }
    
    // navigation property
    public Food Food { get; set; } 
    public int Quantity { get; set; }
}