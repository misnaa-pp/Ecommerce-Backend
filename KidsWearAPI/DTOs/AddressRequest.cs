
using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.DTOs
{
    public class AddressRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(
            @"^(\+91[-\s]?)?[6-9]\d{9}$",
            ErrorMessage = "Enter a valid Indian mobile number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 5,
            ErrorMessage = "Address must be between 5 and 200 characters")]
        public string AddressLine { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal code is required")]
        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "Enter a valid 6-digit Indian PIN code")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        [StringLength(60)]
        public string Country { get; set; } = "India";
    }
}
