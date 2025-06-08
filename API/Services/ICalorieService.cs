using API.DTOs;

public interface ICalorieService
{
    Task<double> CalculateCalorieGoalAsync(GoalDto goalDto);
    Task<BmiAndCalorieDto> CalculateBmiAndCalorieAsync(int userId, int totalCaloriesToday);
    Task<List<MealDto>> CreatePersonalizedMealPlanAsync(int userId, double dailyCalorieTarget);
}