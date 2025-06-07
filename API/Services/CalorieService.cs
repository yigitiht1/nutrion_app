using API.DTOs;
using API.Repositories;

namespace API.Services
{
    public class CalorieService : ICalorieService
    {
        private readonly IUserRepository _userRepository;

        public CalorieService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<CalorieGoalResultDto> CalculateCalorieGoalAsync(GoalDto goalDto)
        {
            var user = await _userRepository.GetUserByIdAsync(goalDto.UserId);
            if (user == null)
                throw new ArgumentException("Kullanıcı bulunamadı.");

            double currentWeight = user.Weight;
            double targetWeight = goalDto.TargetWeight;
            int durationDays = goalDto.TargetDays;

            // 1 kg ≈ 7700 kalori
            double totalCalorieChange = (targetWeight - currentWeight) * 7700;
            double dailyCalorieChange = totalCalorieChange / durationDays;

            return new CalorieGoalResultDto
            {
                DailyCalorieDifference = Math.Round(Math.Abs(dailyCalorieChange), 2),
                IsDeficit = dailyCalorieChange < 0,
                Recommendation = dailyCalorieChange < 0
                    ? "Hedefinize ulaşmak için günlük bu kadar kalori açığı oluşturmalısınız."
                    : "Hedefinize ulaşmak için günlük bu kadar kalori fazlası almalısınız."
            };
        }
    }
}