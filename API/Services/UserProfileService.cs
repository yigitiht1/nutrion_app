using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Data;
public class UserProfileService : IUserProfileService
{
    private readonly AppDbContext _context;

    public UserProfileService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateUserProfileAsync(UserProfileDto dto)
    {
        var profile = new UserProfile
        {
            UserId = dto.UserId,
            Gender = dto.Gender,
            Age = dto.Age,
            Height = dto.Height,
            Weight = dto.Weight
        };

        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
    }

    public async Task<UserProfile?> GetUserProfileByUserIdAsync(int userId)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
    }

    public async Task UpdateUserProfileAsync(int userId, UserProfileDto dto)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);

        if (profile != null)
        {
            profile.Gender = dto.Gender;
            profile.Age = dto.Age;
            profile.Height = dto.Height;
            profile.Weight = dto.Weight;

            await _context.SaveChangesAsync();
        }
    }
}