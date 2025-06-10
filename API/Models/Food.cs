namespace API.Models
{
    public enum MealType
    {
        Breakfast,
        Lunch,
        Dinner,
        Snack
    }

    public class Food
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public int CaloriesPer100g { get; set; }

        public List<FoodMealType> FoodMealTypes { get; set; } = new();
        
    }

    public class FoodMealType
    {
        public int FoodId { get; set; }
        public Food? Food { get; set; }

        public MealType MealType { get; set; }
    }
}