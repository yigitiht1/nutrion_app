using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IFoodRepository
    {
        Task<List<Food>> GetFoodsUnderCaloriesAsync(int maxCalories);
    }
}