using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.DTOs;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserProfile _userProfileService;
        private readonly ICalorieService _calorieService;

        public UserController(IUserService userService, IUserProfile userProfileService, ICalorieService calorieService)
        {
            _userService = userService;
            _userProfileService = userProfileService;
            _calorieService = calorieService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{userId}/bmi-calories")]
        public async Task<IActionResult> GetBmiAndCalorie(int userId, [FromQuery] int totalCaloriesToday)
        {
            try
            {
                var result = await _userService.CalculateBmiAndCalorieAsync(userId, totalCaloriesToday);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
         [HttpGet("{userId}/daily-calorie")]
        public async Task<IActionResult> GetDailyCalorieNeed(int userId)
        {
            try
            {
                var profile = await _userProfileService.GetUserProfileByUserIdAsync(userId);
                if (profile == null)
                    return NotFound(new { message = "Kullanıcı profili bulunamadı." });

                // Basal Metabolic Rate (BMR) + Hafif aktivite çarpanı
                double bmr = 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age + (profile.Gender.ToLower() == "male" ? 5 : -161);
                double dailyCalorieNeed = bmr * 1.2;

                return Ok(new
                {
                    DailyCalorieNeed = Math.Round(dailyCalorieNeed, 2),
                    Message = "Bu, mevcut profilinize göre önerilen günlük kalori ihtiyacıdır."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu.", error = ex.Message });
            }
        }
         [HttpGet("{userId}/profile")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var profile = await _userProfileService.GetUserProfileByUserIdAsync(userId);
            if (profile == null)
                return NotFound(new { message = "Profil bulunamadı." });

            return Ok(profile);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var success = await _userService.RegisterAsync(registerDto);
            if (!success)
                return BadRequest(new { message = "Bu email zaten kayıtlı!" });

            return Ok(new { message = "Kullanıcı başarıyla kaydedildi." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userService.LoginAsync(loginDto.Email, loginDto.Password);
            if (user == null)
                return Unauthorized(new { message = "Geçersiz email veya şifre." });

            return Ok(new { message = "Giriş başarılı.", userId = user.Id });
        }

        [HttpPost("{userId}/calorie-goal")]
        public async Task<IActionResult> CalculateGoalCalories(int userId, [FromBody] GoalDto goalDto)
        {
            if (userId != goalDto.UserId)
                return BadRequest("Kullanıcı ID'si eşleşmiyor.");

            // Mantıklı sınırlar kontrolü
            if (goalDto.TargetWeight <= 0 || goalDto.TargetWeight > 300)
                return BadRequest("Hedef kilo geçersiz.");
            if (goalDto.TargetDays < 7 || goalDto.TargetDays > 365)
                return BadRequest("Hedef gün sayısı 7 ile 365 arasında olmalıdır.");

            try
            {
                var result = await _calorieService.CalculateCalorieGoalAsync(goalDto);

                // Önerilen kalori sınır dışı ise ek kontrol
                if (result < 1200 || result > 5000)
                    return BadRequest("Önerilen kalori değeri sağlıklı aralıkta değil.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpPut("{userId}/profile")]
        public async Task<IActionResult> CreateOrUpdateProfile(int userId, [FromBody] UserProfileDto dto)
        {
            var existingProfile = await _userProfileService.GetUserProfileByUserIdAsync(userId);
            if (existingProfile == null)
            {
                dto.UserId = userId;
                await _userProfileService.CreateUserProfileAsync(dto);
                return Ok(new { message = "Profil oluşturuldu." });
            }
            else
            {
                await _userProfileService.UpdateUserProfileAsync(userId, dto);
                return Ok(new { message = "Profil güncellendi." });
            }
        }
       
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (!result)
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            
            return NoContent(); // 204 döner, başarılı silme
        }
    }
}