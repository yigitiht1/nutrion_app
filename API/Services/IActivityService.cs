using API.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace API.Services
{
    public interface IActivityService
    {
        Task<List<Activity>> GetActivitiesByUserIdAsync(int userId);
        Task<Activity> AddActivityAsync(Activity activity); // Sadece 1 parametre alır
        Task<bool> DeleteActivityAsync(int id);
    }
}