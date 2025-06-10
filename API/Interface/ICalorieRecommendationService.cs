using API.DTOs;


public interface ICalorieRecommendationService
{
    RecommendationDto? GetRecommendationForUser(int userId);
    Task<RecommendationDto?> GetRecommendationForUserAsync(int userId, int totalCaloriesToday);
}