using API.Models;

namespace API.Repositories
{
    public interface IUserRepository
    {
        User? GetUserByEmail(string email);
        void AddUser(User user);
        void SaveChanges();
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserById(int id);
    }
}