namespace API.DTOs
{
 public class MealDto
{
    public int UserId { get; set; }
    public string? MealType { get; set; }
    public DateTime Date { get; set; }
    public List<MealItemDto>? MealItems { get; set; }
}
}