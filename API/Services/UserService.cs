using API.Models;
using API.Repositories;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool Register(User user)
        {
            if (_userRepository.GetUserByEmail(user.Email) != null)
                return false;

            _userRepository.AddUser(user);
            _userRepository.SaveChanges();
            return true;
        }

        public User? Login(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null || !user.VerifyPassword(password)) // Şifre doğrulama
                return null;

            return user;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _userRepository.GetUserById(id);
        }
    }
}