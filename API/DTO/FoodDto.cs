using API.Models;

public class FoodDto
{
    public string Name { get; set; } = string.Empty;
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }

    public List<MealType> MealTypes { get; set; } = new();
    public int PortionGrams { get; set; }
}