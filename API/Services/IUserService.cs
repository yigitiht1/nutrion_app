using API.Models;

namespace API.Services
{
    public interface IUserService
    {
        bool Register(User user);
        User? Login(string email, string password);
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserById(int id);
    }
}