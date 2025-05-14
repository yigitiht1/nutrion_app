namespace API.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public double Height { get; set; } // Boy (cm)
        public double Weight { get; set; } // Kilo (kg)
        public int Age { get; set; } // Yaş
        public string Gender { get; set; } = string.Empty; // Cinsiyet
    }
}