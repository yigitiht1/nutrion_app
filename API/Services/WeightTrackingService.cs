using API.Data;
using Microsoft.EntityFrameworkCore;

public class WeightTrackingService : IWeightTrackingService
{
    private readonly AppDbContext _context;

    public WeightTrackingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UpdateUserWeightAsync(int userId, double newWeight)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId);
        if (profile == null) return false;

        // Mevcut kiloyu güncelle
        profile.Weight = newWeight;

        // Kilo takip kaydını ekle
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
}