public class MealPlanDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<PlannedMealDto> PlannedMeals { get; set; }
}