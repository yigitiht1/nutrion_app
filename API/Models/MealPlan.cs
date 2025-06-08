using System;
using System.Collections.Generic;

namespace API.Models
{
    public class MealPlan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }

        public List<MealPlanItem> Items { get; set; } = new();

        public User User { get; set; }
    }
}