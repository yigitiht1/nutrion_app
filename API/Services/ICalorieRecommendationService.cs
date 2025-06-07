using API.DTOs;
using System.Threading.Tasks;

public interface ICalorieRecommendationService
{
    RecommendationDto? GetRecommendationForUser(int userId);
    Task<RecommendationDto?> GetRecommendationForUserAsync(int userId, int totalCaloriesToday);
}