using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ActivityController : ControllerBase
{
    private readonly IActivityService _service;

    public ActivityController(IActivityService service)
    {
        _service = service;
    }

    // Aktivite ekleme - artık token yok, userId parametre olarak geliyor
    [HttpPost]
    public async Task<IActionResult> AddActivity([FromBody] ActivityDto dto, [FromQuery] int userId)
    {
        if (userId == 0)
            return BadRequest("UserId parametresi zorunludur.");

        var activity = new Activity
        {
            Name = dto.Name,
            CaloriesBurned = dto.CaloriesBurned,
            Date = dto.Date,
            UserId = userId
        };

        await _service.AddActivityAsync(activity);

        var calorieSurplus = await CalculateUserCalorieSurplusAsync(userId);
        var suggestion = GenerateActivitySuggestion(calorieSurplus);

        return Ok(new { message = "Activity added successfully", suggestion });
    }

    // Kullanıcının aktivitelerini getir (userId query'den geliyor)
    [HttpGet]
    public async Task<IActionResult> GetActivities([FromQuery] int userId, [FromQuery] DateTime? date = null)
    {
        if (userId == 0)
            return BadRequest("UserId parametresi zorunludur.");

        var activities = await _service.GetActivitiesByUserIdAsync(userId);

        if (date.HasValue)
        {
            activities = activities.Where(a => a.Date.Date == date.Value.Date).ToList();
        }

        return Ok(activities);
    }

    // Kullanıcının kalori fazlasını hesapla (örnek)
    private async Task<double> CalculateUserCalorieSurplusAsync(int userId)
    {
        // Burada gerçek kalori alımını kullanıcı verisinden almalısın.
        double calorieIntake = 2500;

        var activities = await _service.GetActivitiesByUserIdAsync(userId);
        double caloriesBurned = activities.Sum(a => a.CaloriesBurned);

        return calorieIntake - caloriesBurned; // Pozitifse kalori fazlası var demek
    }

    // Kalori fazlasına göre öneri üret
    private string GenerateActivitySuggestion(double calorieSurplus)
    {
        if (calorieSurplus < 500)
            return "Kalori fazlasınız düşük, günlük aktivitelerinize devam edin.";

        if (calorieSurplus < 1000)
            return "Yaklaşık 500 kalori fazlanız var. 30 dakika hızlı yürüyüş öneriyoruz.";

        if (calorieSurplus < 2000)
            return "Yaklaşık 1000 kalori fazlanız var. 1 saat koşu veya 2 saat yürüyüş yapabilirsiniz.";

        return "Yüksek kalori fazlası tespit edildi. Günlük egzersiz sürenizi artırmayı düşünebilirsiniz.";
    }
}