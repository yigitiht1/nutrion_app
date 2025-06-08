namespace API.Models
{
    public class MealPlanItem
    {
        public int Id { get; set; }
        public int MealPlanId { get; set; }
        public int FoodId { get; set; }
        public int Quantity { get; set; }

        public MealPlan MealPlan { get; set; }
        public Food Food { get; set; }
    }
}