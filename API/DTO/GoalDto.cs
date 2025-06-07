namespace API.DTOs
{
    public class GoalDto
    {
        public int UserId { get; set; }
        public double TargetWeight { get; set; } // hedef kilo
        public int TargetDays { get; set; }      // hedefe ulaşmak istenen süre (gün)
    }
}