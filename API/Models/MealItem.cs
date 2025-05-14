namespace API.Models
{
public class MealItem
{
    public int Id { get; set; }

    public int MealId { get; set; }
    public Meal Meal { get; set; }

    public int FoodId { get; set; }
    public Food Food { get; set; }

    public int Quantity { get; set; } = 1;
}
}