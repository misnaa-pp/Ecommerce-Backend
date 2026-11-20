
using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.DTOs
{
    public class UpdateProfileRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [RegularExpression(
            @"^$|^[6-9]\d{9}$",
            ErrorMessage = "Enter a valid 10-digit mobile number")]
        public string? Phone { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }
    }
}
