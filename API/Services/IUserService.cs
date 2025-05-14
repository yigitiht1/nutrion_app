using API.DTOs;
using API.Models;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<User?> LoginAsync(string email, string password);
    }
}