using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Models;

namespace API.Entities
{
    public class Goal
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public double TargetWeight { get; set; }
        public int TargetDays { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public int RequiredCalorieDifference { get; set; }

       public int RemainingDays =>
        Math.Max(0, TargetDays - (DateTime.UtcNow.Date - StartDate.Date).Days);

                                

        public virtual User User { get; set; }

    }
}