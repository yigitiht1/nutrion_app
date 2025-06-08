using API.Data;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ActivityController : ControllerBase
{
    private readonly AppDbContext _context;

    public ActivityController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddActivity([FromBody] ActivityDto dto)
    {
        var activity = new Activity
        {
            UserId = dto.UserId,
            Type = dto.Type,
            DurationInMinutes = dto.DurationInMinutes,
            CaloriesBurned = dto.CaloriesBurned,
            Date = dto.Date
        };

        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Aktivite kaydedildi." });
    }
}