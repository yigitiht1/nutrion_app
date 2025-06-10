using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.DTOs;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfile _userProfileService;

        public UserProfileController(IUserProfile userProfileService)
        {
            _userProfileService = userProfileService;
        }
        
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var profile = await _userProfileService.GetUserProfileByUserIdAsync(userId);
            if (profile == null)
                return NotFound(new { message = "Profil bulunamadı." });

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileDto dto)
        {
            await _userProfileService.CreateUserProfileAsync(dto);
            return Ok(new { message = "Profil oluşturuldu." });
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UserProfileDto dto)
        {
            await _userProfileService.UpdateUserProfileAsync(userId, dto);
            return Ok(new { message = "Profil güncellendi." });
        }
    }
}