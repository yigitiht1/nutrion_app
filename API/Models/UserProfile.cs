namespace API.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public double Height { get; set; } // cm
        public double Weight { get; set; } // kg

        public User? User { get; set; } // Navigation property
    }
}