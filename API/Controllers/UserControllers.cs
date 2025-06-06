using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserProfile _userProfileService;

        public UserController(IUserService userService, IUserProfile userProfileService)
        {
            _userService = userService;
            _userProfileService = userProfileService;
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
        
    }
}