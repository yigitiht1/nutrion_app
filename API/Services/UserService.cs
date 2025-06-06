using API.DTOs;
using API.Models;
using API.Repositories;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return false;

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Height = dto.Height,
                Weight = dto.Weight,
                Age = dto.Age,
                Gender = dto.Gender
            };

            user.SetPassword(dto.Password);

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !user.VerifyPassword(password))
                return null;

            return user;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }
        
        public async Task<BmiAndCalorieDto> CalculateBmiAndCalorieAsync(int userId, int totalCaloriesConsumedToday)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Kullanıcı bulunamadı.");

            return CalculateBmiAndCalorie(user, totalCaloriesConsumedToday);
        }

        public BmiAndCalorieDto CalculateBmiAndCalorie(User user, int totalCaloriesToday)
        {
            var heightMeters = user.Height / 100.0;
            var bmi = user.Weight / (heightMeters * heightMeters);

            string bmiCategory = bmi switch
            {
                <= 18.5 => "Zayıf",
                > 18.5 and <= 24.9 => "Normal",
                > 24.9 and <= 29.9 => "Fazla Kilolu",
                _ => "Obez"
            };

            double bmr = user.Gender.ToLower() switch
            {
                "male" or "erkek" => 88.362 + (13.397 * user.Weight) + (4.799 * user.Height) - (5.677 * user.Age),
                _ => 447.593 + (9.247 * user.Weight) + (3.098 * user.Height) - (4.330 * user.Age)
            };

            int recommendedCalories = (int)(bmr * 1.2); // Sedanter yaşam için aktivite çarpanı

            bool isExceeded = totalCaloriesToday > recommendedCalories;

            string advice = isExceeded
                ? "Bugün kalori ihtiyacınızı aştınız. Daha dengeli beslenmeye çalışın."
                : (totalCaloriesToday == recommendedCalories
                    ? "Kalori ihtiyacınızı tam karşılıyorsunuz. İyi iş çıkardınız!"
                    : "Kalori ihtiyacınızın altında kalıyorsunuz. Yeterli beslenmeye dikkat edin.");

            return new BmiAndCalorieDto
            {
                BMI = Math.Round(bmi, 2),
                BMICategory = bmiCategory,
                RecommendedCalories = recommendedCalories,
                TotalCaloriesToday = totalCaloriesToday,
                IsCalorieLimitExceeded = isExceeded,
                AdviceMessage = advice
            };
        }
    }
}