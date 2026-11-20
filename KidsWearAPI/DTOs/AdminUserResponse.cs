
namespace KidsWearAPI.DTOs
{
    public class AdminUserResponse
    {
        public int UserId { get; set; }

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public string? Phone { get; set; }

        public string Role { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public bool IsBlocked { get; set; }
    }
}
