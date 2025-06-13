using API.DTOs;
using API.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
            var userProfile = new UserProfile
            {
                UserId = dto.UserId,
                Height = dto.Height,
                Weight = dto.Weight,
                Age = dto.Age,
                Gender = dto.Gender
            };

            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
        }

        public async Task<UserProfile?> GetUserProfileByUserIdAsync(int userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
        }

        public async Task UpdateUserProfileAsync(int userId, UserProfileDto dto)
        {
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
            if (userProfile == null)
            {
                Console.WriteLine("UserProfile bulunamadı.");
                return;
            }

            userProfile.Height = dto.Height;
            userProfile.Weight = dto.Weight;
            userProfile.Age = dto.Age;
            userProfile.Gender = dto.Gender;

            var changes = await _context.SaveChangesAsync();
            Console.WriteLine($"{changes} kayıt güncellendi.");
        }
        
    }
}