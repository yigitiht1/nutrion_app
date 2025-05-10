using API.DTOs;
using API.Models;

public interface IUserProfileService
{
    Task CreateUserProfileAsync(UserProfileDto dto);
    Task<UserProfile?> GetUserProfileByUserIdAsync(int userId);
    Task UpdateUserProfileAsync(int userId, UserProfileDto dto);
}