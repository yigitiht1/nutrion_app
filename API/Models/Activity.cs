namespace API.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double CaloriesBurnedPerMinute { get; set; }
    }
}