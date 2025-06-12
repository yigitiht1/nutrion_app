namespace API.DTOs
{
    public class GoalDto
    {
        public int UserId { get; set; }
        public double TargetWeight { get; set; }
        public int TargetDays { get; set; }
        public DateTime StartDate { get; set; }
        public int RemainingDays =>
    Math.Max(0, TargetDays - (DateTime.UtcNow.Date - StartDate.Date).Days);    
    }
}