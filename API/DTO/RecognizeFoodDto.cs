namespace API.DTOs
{
public class RecognizeFoodDto
{
    public int UserId { get; set; }
    public string? MealType { get; set; }
    public string? FoodName { get; set; }
    public int Quantity { get; set; } = 1;
}
}