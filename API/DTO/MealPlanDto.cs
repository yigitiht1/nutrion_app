namespace API.DTOs
{
    public class MealPlanDto
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationDays { get; set; }
    }
}