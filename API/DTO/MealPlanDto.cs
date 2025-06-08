using System;
using System.Collections.Generic;
using API.Models;

namespace API.DTOs
{
    public class MealPlanDto
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<MealPlanItemDto> Items { get; set; } = new();
    }

    public class MealPlanItemDto
    {
        public DateTime Date { get; set; }
        public MealType MealType { get; set; }
        public int FoodId { get; set; }
        public double Quantity { get; set; }
    }
}