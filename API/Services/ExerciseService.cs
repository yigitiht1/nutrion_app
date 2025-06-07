using API.Models;
using API.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

using API.Models;
using API.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository _repository;

        public ExerciseService(IExerciseRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Exercise>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Exercise?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<Exercise> AddAsync(Exercise exercise)
        {
            return _repository.AddAsync(exercise);
        }

        public async Task<Exercise?> UpdateAsync(Exercise exercise)
        {
            var existing = await _repository.GetByIdAsync(exercise.Id);
            if (existing == null) return null;

            existing.Name = exercise.Name;
            existing.CaloriesBurnedPerMinute = exercise.CaloriesBurnedPerMinute;

            await _repository.UpdateAsync(existing);

            return existing;
        }

        public Task<bool> DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }
    }
}