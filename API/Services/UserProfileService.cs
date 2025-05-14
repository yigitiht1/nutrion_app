using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserProfileService : IUserProfile
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
                Height = dto.Height,
                Weight = dto.Weight,
                Age = dto.Age,
                Gender = dto.Gender
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        public async Task<UserProfile?> GetUserProfileByUserIdAsync(int userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task UpdateUserProfileAsync(int userId, UserProfileDto dto)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
                throw new Exception("Profil bulunamadı.");

            profile.Height = dto.Height;
            profile.Weight = dto.Weight;
            profile.Age = dto.Age;
            profile.Gender = dto.Gender;

            await _context.SaveChangesAsync();
        }
    }
}