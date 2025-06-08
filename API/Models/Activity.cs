using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // Örn: "Yürüyüş", "Koşu"

        [Required]
        public double CaloriesBurned { get; set; } // Yaktığı kalori

        [Required]
        public DateTime Date { get; set; }

        // Kullanıcı ile ilişki
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}