using API.DTOs;

public interface ICalorieService
{
    Task<string> CalculateCalorieGoalAsync(GoalDto goalDto);
    Task<BmiAndCalorieDto> CalculateBmiAndCalorieAsync(int userId, int totalCaloriesToday);
    Task<List<MealDto>> CreatePersonalizedMealPlanAsync(int userId, double dailyCalorieTarget);
}