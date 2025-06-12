public interface IWeightTrackingService
{
    Task<bool> UpdateUserWeightAsync(int userId, double newWeight);
    Task<List<WeightHistoryDto>> GetWeightHistoryAsync(int userId);
}