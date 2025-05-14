namespace API.Models
{
   public class Meal
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string MealType { get; set; } // "Kahvaltı", "Öğle", vs.
    public DateTime Date { get; set; }

    public List<MealItem> MealItems { get; set; } = new();
}
}