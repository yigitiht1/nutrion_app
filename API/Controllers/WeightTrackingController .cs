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
    public async Task<IActionResult> UpdateWeight([FromBody] UpdateWeightDto dto)
    {
        var result = await _weightTrackingService.UpdateUserWeightAsync(dto.UserId, dto.NewWeight);
        if (!result) return BadRequest("Kilo güncellenemedi.");
        return Ok("Kilo başarıyla güncellendi.");
    }

}
