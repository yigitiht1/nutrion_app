namespace API.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public string MealType { get; set; } = string.Empty; // Örn: Kahvaltı, Öğle
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public List<Food> Foods { get; set; } = new();
    }
}