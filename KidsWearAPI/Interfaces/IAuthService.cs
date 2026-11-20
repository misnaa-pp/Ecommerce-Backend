
using KidsWearAPI.DTOs;

namespace KidsWearAPI.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterRequest request);

        Task<AuthResponse?> Login(LoginRequest request);
        Task<AuthResponse?> RefreshAccessToken(RefreshTokenRequest request);

        Task<object?> GetMyProfile(int userId);
        Task<bool> Logout(RefreshTokenRequest request);
        Task<string?> UpdateMyProfile(int userId,UpdateProfileRequest request);}
}
