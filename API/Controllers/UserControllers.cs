using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using API.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }   

           [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            var userDtos = users.Select(u => new UserDto
            {
                Name = u.Name,
                Email = u.Email
            }).ToList();

            return Ok(userDtos);
        }

          [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(new UserDto { Name = user.Name, Email = user.Email });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email
            };
            user.SetPassword(registerDto.Password);

            if (_userService.Register(user))
                return Ok(new { message = "Kullanıcı başarıyla kaydedildi." });

            return BadRequest(new { message = "Bu email zaten kayıtlı!" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var loggedInUser = _userService.Login(loginDto.Email, loginDto.Password);
            if (loggedInUser == null)
                return Unauthorized(new { message = "Geçersiz email veya şifre." });

            return Ok(new { message = "Giriş başarılı.", userId = loggedInUser.Id });
        }

     
        

      
    }
}