using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace API.Services
{
    public interface IExerciseService
    {
        Task<IEnumerable<Exercise>> GetAllAsync();
        Task<Exercise?> GetByIdAsync(int id);
        Task<Exercise> AddAsync(Exercise exercise);
        Task<Exercise?> UpdateAsync(Exercise exercise);
        Task<bool> DeleteAsync(int id);
    }
}