using API.Entities;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data{
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
        public DbSet<Activity> Activities { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<MealPlanItem> MealPlanItems { get; set; }

        
       
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<FoodMealType>()
        .HasKey(fm => new { fm.FoodId, fm.MealType });

    modelBuilder.Entity<FoodMealType>()
        .HasOne(fm => fm.Food)
        .WithMany(f => f.FoodMealTypes)
        .HasForeignKey(fm => fm.FoodId);

    // MealPlan ile MealPlanItem arasında bire çok ilişki
    modelBuilder.Entity<MealPlan>()
        .HasMany(mp => mp.MealPlanItems)
        .WithOne()
        .HasForeignKey(mpi => mpi.MealPlanId)
        .OnDelete(DeleteBehavior.Cascade);
}

  
}
}