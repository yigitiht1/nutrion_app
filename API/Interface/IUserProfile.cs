using API.DTOs;



namespace API.Services
{
    public interface IUserProfile
    {
        Task CreateUserProfileAsync(UserProfileDto dto);
        Task<UserProfile?> GetUserProfileByUserIdAsync(int userId);
        Task UpdateUserProfileAsync(int userId, UserProfileDto dto);
    }
}