using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IExerciseRepository
    {
        Task<IEnumerable<Exercise>> GetAllAsync();
        Task<Exercise?> GetByIdAsync(int id);
        Task<Exercise> AddAsync(Exercise exercise);
        Task UpdateAsync(Exercise exercise);
        Task<bool> DeleteAsync(int id);
    }
}