using System.ComponentModel.DataAnnotations;
using API.Models;

namespace API.Models
{
    public class UserExercise
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        public int DurationMinutes { get; set; }
        public DateTime Date { get; set; }
    }
}
    