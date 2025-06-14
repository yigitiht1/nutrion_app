using API.Entities;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<MealItem> MealItems { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<FoodMealType> FoodMealTypes { get; set; }
       public DbSet<WeightTracking> WeightTrackings { get; set; }

        public DbSet<PlannedMeal> PlannedMeals { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<MealPlanItem> MealPlanItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<FoodMealType>()
                .HasKey(fm => new { fm.FoodId, fm.MealType });


            modelBuilder.Entity<FoodMealType>()
                .HasOne(fm => fm.Food)
                .WithMany(f => f.FoodMealTypes)
                .HasForeignKey(fm => fm.FoodId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<MealPlan>()
                .HasMany(mp => mp.MealPlanItems)
                .WithOne()
                .HasForeignKey(mpi => mpi.MealPlanId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<MealPlan>()
                .HasMany(mp => mp.PlannedMeals)
                .WithOne(pm => pm.MealPlan)
                .HasForeignKey(pm => pm.MealPlanId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<Meal>()
                .HasMany(m => m.MealItems)
                .WithOne(mi => mi.Meal)
                .HasForeignKey(mi => mi.MealId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}