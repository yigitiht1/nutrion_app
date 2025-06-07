
using API.DTOs;
using API.Repositories;

using API.DTOs;
using API.Repositories;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public class CalorieService : ICalorieService
    {
        private readonly IUserRepository _userRepository;
        private readonly IExerciseRepository _exerciseRepository;

        public CalorieService(IUserRepository userRepository, IExerciseRepository exerciseRepository)
        {
            _userRepository = userRepository;
            _exerciseRepository = exerciseRepository;
        }

        public async Task<CalorieGoalResultDto> CalculateCalorieGoalAsync(GoalDto goalDto)
        {
            var user = await _userRepository.GetUserByIdAsync(goalDto.UserId);
            if (user == null)
                throw new ArgumentException("Kullanıcı bulunamadı.");

            double currentWeight = user.Weight;
            double targetWeight = goalDto.TargetWeight;
            int durationDays = goalDto.TargetDays;

            double totalCalorieChange = (targetWeight - currentWeight) * 7700; // 1 kg ~ 7700 kalori
            double dailyCalorieChange = totalCalorieChange / durationDays;

            var result = new CalorieGoalResultDto
            {
                DailyCalorieDifference = Math.Round(Math.Abs(dailyCalorieChange), 2),
                IsDeficit = dailyCalorieChange < 0,
                Recommendation = dailyCalorieChange < 0
                    ? "Hedefinize ulaşmak için günlük bu kadar kalori açığı oluşturmalısınız."
                    : "Hedefinize ulaşmak için günlük bu kadar kalori fazlası almalısınız."
            };

            if (dailyCalorieChange < 0) // Kalori açığı varsa egzersiz öner
            {
                var exercises = await _exerciseRepository.GetAllAsync();

                foreach (var ex in exercises)
                {
                    double minutesNeeded = Math.Abs(dailyCalorieChange) / ex.CaloriesBurnedPerMinute;
                    result.ExerciseRecommendations.Add(new ExerciseRecommendationDto
                    {
                        ExerciseName = ex.Name,
                        MinutesPerDay = Math.Round(minutesNeeded, 1)
                    });
                }
            }

            return result;
        }
    }
}