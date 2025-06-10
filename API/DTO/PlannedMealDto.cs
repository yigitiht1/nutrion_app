using API.Models;

public class PlannedMealDto
{
    public string Day { get; set; }  // Örneğin "Monday" ya da "2024-06-10"
    public MealType MealType { get; set; }
    public List<FoodDto> Foods { get; set; }  // Bu satırı ekle
}