using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IActivityRepository
{
    Task<List<Activity>> GetRecommendedActivitiesAsync(int targetCaloriesToBurn);
}
}