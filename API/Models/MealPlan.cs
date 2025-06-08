using System;
using System.Collections.Generic;

namespace API.Models
{
    public class MealPlan
    {
        public int Id { get; set; }
        public int UserId { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<MealPlanItem> Items { get; set; } = new();
    }

    public class MealPlanItem
    {
        public int Id { get; set; }
        public int MealPlanId { get; set; }
        public MealPlan MealPlan { get; set; }

        public DateTime Date { get; set; }
        public MealType MealType { get; set; }

        public int FoodId { get; set; }
        public Food Food { get; set; }

        public double Quantity { get; set; } // porsiyon/gram gibi
    }
}