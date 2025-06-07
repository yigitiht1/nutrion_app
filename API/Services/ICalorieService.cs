using API.DTOs;
using System.Threading.Tasks;

public interface ICalorieService
{
    Task<CalorieGoalResultDto> CalculateCalorieGoalAsync(GoalDto goalDto);
}