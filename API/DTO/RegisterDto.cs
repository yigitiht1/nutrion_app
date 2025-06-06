namespace API.DTOs
{
    public class RegisterDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public double Height { get; set; }  // cm cinsinden
        public double Weight { get; set; }  // kg cinsinden
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
    }
}