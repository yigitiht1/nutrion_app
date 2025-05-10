using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data{
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
}
}