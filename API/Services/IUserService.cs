using API.DTOs;
using API.Models;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<User?> LoginAsync(string email, string password);
        Task<User?> GetUserByIdAsync(int userId);
        BmiAndCalorieDto CalculateBmiAndCalorie(User user, int totalCaloriesToday);
        Task<BmiAndCalorieDto> CalculateBmiAndCalorieAsync(int userId, int totalCaloriesConsumedToday);
    }
}