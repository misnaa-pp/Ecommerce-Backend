using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";
        public String? Phone {  get; set; }
        public String? Address { get; set; }
        public string Role { get; set; } = "User";
        public bool IsBlocked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
