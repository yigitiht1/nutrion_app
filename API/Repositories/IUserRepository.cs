using API.Models;
using System.Collections.Generic;


namespace API.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
         void DeleteUser(User user);
        Task SaveChangesAsync();
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersWithProfileAsync();
    }
}