using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double CaloriesBurnedPerMinute { get; set; }
         public ICollection<UserExercise> UserExercises { get; set; } = new List<UserExercise>();
    }
}