namespace API.DTOs
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
    }
}