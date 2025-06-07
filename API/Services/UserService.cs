using API.DTOs;
using API.Models;
using API.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfile _userProfileService;

        public UserService(IUserRepository userRepository, IUserProfile userProfileService)
        {
            _userRepository = userRepository;
            _userProfileService = userProfileService;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Height = u.Height,
                Weight = u.Weight,
                Age = u.Age,
                Gender = u.Gender
            });
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return false; // email zaten kayıtlı

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password), // Şifre hashleme metodunu implement et
                Height = dto.Height,
                Weight = dto.Weight,
                Age = dto.Age,
                Gender = dto.Gender
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            // Kayıt başarılı, profil oluştur
            var userProfileDto = new UserProfileDto
            {
                UserId = user.Id,
                Height = (int)dto.Height,
                Weight = (int)dto.Weight,
                Age = dto.Age,
                Gender = dto.Gender
            };

            await _userProfileService.CreateUserProfileAsync(userProfileDto);

            return true;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public BmiAndCalorieDto CalculateBmiAndCalorie(User user, int totalCaloriesToday)
        {
            // BMI hesaplama ve kalori önerisi örneği
            var heightMeters = user.Height / 100;
            var bmi = user.Weight / (heightMeters * heightMeters);
            string category;
            if (bmi < 18.5) category = "Zayıf";
            else if (bmi < 25) category = "Normal";
            else if (bmi < 30) category = "Fazla Kilolu";
            else category = "Obez";

            int recommendedCalories = 2000; // basit sabit öneri, gerçek formül ile değiştirilebilir

            return new BmiAndCalorieDto
            {
                BMI = bmi,
                BMICategory = category,
                RecommendedCalories = recommendedCalories,
                TotalCaloriesToday = totalCaloriesToday,
                IsCalorieLimitExceeded = totalCaloriesToday > recommendedCalories,
                AdviceMessage = totalCaloriesToday > recommendedCalories ? "Kalori limitinizi aştınız!" : "Kalori limitiniz uygun."
            };
        }

        public async Task<BmiAndCalorieDto> CalculateBmiAndCalorieAsync(int userId, int totalCaloriesConsumedToday)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new System.ArgumentException("Kullanıcı bulunamadı");

            return CalculateBmiAndCalorie(user, totalCaloriesConsumedToday);
        }

        // Örnek şifre hashleme (basit)
        private string HashPassword(string password)
        {
            // Gerçek projede güvenli bir hashleme algoritması kullan
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Gerçek projede güvenli doğrulama yap
            var hashOfInput = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
            return hashOfInput == hashedPassword;
        }
    }
}