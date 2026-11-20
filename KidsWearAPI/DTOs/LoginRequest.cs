

using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(254, ErrorMessage = "Email is too long")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
        public string Password { get; set; } = string.Empty;
    }
}

