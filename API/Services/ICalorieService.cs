using API.DTOs;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ICalorieService
    {
        Task<CalorieGoalResultDto> CalculateCalorieGoalAsync(GoalDto goalDto);
    }
}