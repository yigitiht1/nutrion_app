using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Yeni alanlar
        public double Height { get; set; } // cm
        public double Weight { get; set; } // kg
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;

        // Şifre hashleme
        public void SetPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            PasswordHash = Convert.ToBase64String(hashedBytes);
        }

        // Şifre doğrulama
        public bool VerifyPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return PasswordHash == Convert.ToBase64String(hashedBytes);
        }
        
        

    }
}