namespace API.DTOs
{
    public class ActivityDto
    {
        public int UserId { get; set; }
        public string Type { get; set; }
        public double DurationInMinutes { get; set; }
        public double CaloriesBurned { get; set; } // Mobil ya da AI'den hesaplanabilir
        public DateTime Date { get; set; }
    }
}