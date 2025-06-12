using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WeightTrackingController : ControllerBase
{
    private readonly IWeightTrackingService _weightTrackingService;

    public WeightTrackingController(IWeightTrackingService weightTrackingService)
    {
        _weightTrackingService = weightTrackingService;
    }
    [HttpGet("weight-history/{userId}")]
    public async Task<IActionResult> GetWeightHistory(int userId)
    {
        var history = await _weightTrackingService.GetWeightHistoryAsync(userId);

        if (history == null || !history.Any())
            return NotFound("Kilo geçmişi bulunamadı.");

        return Ok(history);
    }

    [HttpPost("update-weight")]
    public async Task<IActionResult> UpdateWeight([FromBody] UpdateWeightRequest request)
    {
        if (request.NewWeight <= 0)
            return BadRequest("Geçersiz kilo değeri.");

        var success = await _weightTrackingService.UpdateUserWeightAsync(request.UserId, request.NewWeight);

        if (!success)
            return NotFound("Kullanıcı profili bulunamadı.");

        return Ok("Kilo güncellendi.");
    }

}

public class UpdateWeightRequest
{
    public int UserId { get; set; }
    public double NewWeight { get; set; }
}