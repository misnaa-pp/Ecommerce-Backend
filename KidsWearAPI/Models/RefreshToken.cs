
using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.Models
{
    public class RefreshToken
    {
        [Key]
        public int RefreshTokenId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string TokenHash { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        public User User { get; set; } = null!;
    }
}
