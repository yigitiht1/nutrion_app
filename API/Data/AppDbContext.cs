using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealItem> MealItems { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<UserExercise> UserExercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<UserExercise>()
        .HasOne(ue => ue.User)
        .WithMany(u => u.UserExercises)
        .HasForeignKey(ue => ue.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<UserExercise>()
        .HasOne(ue => ue.Exercise)
        .WithMany(e => e.UserExercises)
        .HasForeignKey(ue => ue.ExerciseId)
        .OnDelete(DeleteBehavior.Cascade);
}   
}
}