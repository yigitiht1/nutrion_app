using API.Models;

public class WeightTracking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Weight { get; set; }
    public DateTime Date { get; set; }

    public User User { get; set; } //ilişki
}