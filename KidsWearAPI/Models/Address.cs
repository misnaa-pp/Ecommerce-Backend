using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string AddressLine { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        public string PostalCode { get; set; } = string.Empty;

        public string Country { get; set; } = "India";

        public bool IsDefault { get; set; } = false;

        public User User { get; set; } = null!;
    }
}