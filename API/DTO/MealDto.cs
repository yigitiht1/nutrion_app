namespace API.DTOs
{
    public class MealDto
    {
        public string MealType { get; set; } = string.Empty;
        public int UserId { get; set; }
        public List<int> FoodIds { get; set; } = new();
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}