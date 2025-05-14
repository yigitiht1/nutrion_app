using API.DTOs;
using API.Models;
using API.Repositories;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return false;

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email
            };
            user.SetPassword(dto.Password);  // Şifreyi hash'le

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !user.VerifyPassword(password))  // Şifreyi doğrula
                return null;

            return user;
        }
        
    }
}