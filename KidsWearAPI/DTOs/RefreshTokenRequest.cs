
using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.DTOs
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = "";
    }
}
