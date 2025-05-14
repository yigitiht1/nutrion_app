namespace API.DTOs
{
    public class UserDetailsDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public UserProfileDto? Profile { get; set; }
    }
}