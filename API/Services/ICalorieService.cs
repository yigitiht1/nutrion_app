using API.DTOs;

public interface ICalorieService
{
    Task<string> CalculateCalorieGoalAsync(GoalDto goalDto);
    Task<BmiAndCalorieDto> CalculateBmiAndCalorieAsync(int userId, int totalCaloriesToday);
}