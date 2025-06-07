using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class UserProfile
    {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public int Height { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }

        public User? User { get; set; }

        // Hesaplama fonksiyonu ekleyebilirsiniz
        public decimal CalculateCaloricNeeds()
        {
            // Burada kalori hesaplaması yapabilirsiniz.
            return 2000m; // Basit bir değer.
        }
    }
}