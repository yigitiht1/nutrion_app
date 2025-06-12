using API.Data;
using Microsoft.EntityFrameworkCore;

public class WeightTrackingService : IWeightTrackingService
{
    private readonly AppDbContext _context;

    public WeightTrackingService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<WeightHistoryDto>> GetWeightHistoryAsync(int userId)
    {
        var history = await _context.WeightTrackings
            .Where(w => w.UserId == userId)
            .OrderBy(w => w.Date)
            .Select(w => new WeightHistoryDto
            {
                Date = w.Date,
                Weight = w.Weight
            })
            .ToListAsync();

        return history;
    }
    public async Task<bool> UpdateUserWeightAsync(int userId, double newWeight)
{
    try
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId);
        if (profile == null) return false;

        profile.Weight = newWeight;

        var weightRecord = new WeightTracking
        {
            UserId = userId,
            Weight = newWeight,
            Date = DateTime.UtcNow
        };
        _context.WeightTrackings.Add(weightRecord);

        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Weight update error: " + ex.Message);
        return false;
    }
}
}