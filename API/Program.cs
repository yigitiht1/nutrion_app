using API.Data;
using API.Models;
using API.Repositories;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MySQL bağlantısını yapılandır
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33))
    ));

// Bağımlılıkları ekle
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserProfile, UserProfileService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IMealService, MealService>();
builder.Services.AddScoped<ICalorieService, CalorieService>();
builder.Services.AddScoped<ICalorieRecommendationService, CalorieRecommendationService>();
builder.Services.AddScoped<IActivityService, ActivityService>();




builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();
app.MapControllers();

app.Run();