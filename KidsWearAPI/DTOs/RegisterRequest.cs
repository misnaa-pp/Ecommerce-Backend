

using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 50 characters")]
        [RegularExpression(
            @"^[\p{L}\p{M}]+(?:[ .'-][\p{L}\p{M}]+)*$",
            ErrorMessage = "Name contains invalid characters")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(254, ErrorMessage = "Email is too long")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be between 8 and 100 characters")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
        public string Password { get; set; } = "";

        [RegularExpression(
            @"^$|^[6-9]\d{9}$",
            ErrorMessage = "Enter a valid 10-digit Indian mobile number")]
        public string? Phone { get; set; }
    }
}
