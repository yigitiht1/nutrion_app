namespace API.DTOs
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
    }
}