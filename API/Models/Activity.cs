using API.Models;

public class Activity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } // örneğin: "koşu", "yüzme"
    public double DurationInMinutes { get; set; } 
    public double CaloriesBurned { get; set; }
    public DateTime Date { get; set; }

    public User User { get; set; }
}