using System.ComponentModel.DataAnnotations.Schema;
using API.Models;

public class UserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } 
    public double Height { get; set; }
    public double Weight { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }

    public double TargetWeight { get; set; }   
    public int TargetDays { get; set; }  
    public int  CalorieDifference { get; set; }
         
    [ForeignKey("UserId")]
    public User  User { get; set; }









    public double CalculateBMI()
    {
        return Weight / Math.Pow(Height / 100, 2);
    }

public string GetBMICategory(double bmi)
{
    return bmi switch
    {
        < 18.5 => "Zayıf",
        < 25 => "Normal",
        < 30 => "Fazla kilolu",
        _ => "Obez"
    };
}

public double CalculateRecommendedCalories()
{
    if (Weight <= 0 || Height <= 0 || Age <= 0)
        return 0;

    string gender = Gender?.ToLower();
    if (gender != "male" && gender != "female")
        gender = "male"; // default olarak erkek kabul ediliyor

    // 1. BMR Hesabı
    double bmr = gender == "male"
        ? 10 * Weight + 6.25 * Height - 5 * Age + 5
        : 10 * Weight + 6.25 * Height - 5 * Age - 161;

    // 2. Aktivite çarpanı: Ortalama aktiflik = 1.5
    double maintenanceCalories = bmr * 1.5;

    // 3. Hedef kilo bilgisi varsa
    if (TargetWeight > 0 && TargetDays >= 7)
    {
        double calorieDiff = (Weight - TargetWeight) * 7700 / TargetDays;

        // Güvenli sınır: Günlük max 1100 kalori fark
        if (Math.Abs(calorieDiff) > 1100)
        {
            calorieDiff = 1100 * Math.Sign(calorieDiff); // yönü koruyarak sınırlıyoruz
        }

        double recommendedCalories = maintenanceCalories - calorieDiff;

        // Minimum güvenli sınır: Kadınlar için 1200, erkekler için 1500 kalori altına düşmesin
        double minimumSafeCalories = gender == "male" ? 1500 : 1200;
        if (recommendedCalories < minimumSafeCalories)
            recommendedCalories = minimumSafeCalories;

        return recommendedCalories;
    }

    // Hedef yoksa bakım kalorisi önerilir
    return maintenanceCalories;
}
}