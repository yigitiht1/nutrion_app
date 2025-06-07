using API.Models;

public class UserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }

    public double TargetWeight { get; set; }   // EKLENDİ
    public int TargetDays { get; set; }        // EKLENDİ

    public User User { get; set; }

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
    double bmr = Gender.ToLower() == "male"
        ? 10 * Weight + 6.25 * Height - 5 * Age + 5
        : 10 * Weight + 6.25 * Height - 5 * Age - 161;

    double maintenanceCalories = bmr * 1.5;

    if (TargetWeight > 0 && TargetDays > 0)
    {
        double calorieDiff = (Weight - TargetWeight) * 7700 / TargetDays;
        return maintenanceCalories - calorieDiff;
    }

    return maintenanceCalories;
}
}