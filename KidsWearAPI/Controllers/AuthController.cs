

using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;

namespace KidsWearAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase

    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            var result = await _authService.Register(request);

            if (result == "Email already registered")
            {
                return BadRequest(new
                {
                    message = result
                });
            }

            return Ok(new
            {
                message = result
            });
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.Login(request);

            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
            }

            return Ok(new
            {
                message = "Login successful",
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken
            });
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            RefreshTokenRequest request)
        {
            var result = await _authService.RefreshAccessToken(request);

            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid or expired refresh token"
                });
            }

            return Ok(new
            {
                message = "Token refreshed successfully",
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken
            });
        }




        [HttpGet("profile")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _authService.GetMyProfile(
                int.Parse(userId)
            );

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            return Ok(user);
        }


        [HttpPut("profile")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateMyProfile(
            UpdateProfileRequest request)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _authService.UpdateMyProfile(
                int.Parse(userId),
                request
            );

            if (result == "User not found")
            {
                return NotFound(new { message = result });
            }

            if (result == "Email already registered")
            {
                return BadRequest(new { message = result });
            }

            var updatedUser = await _authService.GetMyProfile(
                int.Parse(userId)
            );

            return Ok(updatedUser);
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRequest request)
        {
            var result = await _authService.Logout(request);

            if (!result)
            {
                return Unauthorized(new
                {
                    message = "Invalid or already revoked refresh token"
                });
            }

            return Ok(new
            {
                message = "Logout successful"
            });
        }


        [HttpGet("admin-profile")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminProfile()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (userId == null)
                return Unauthorized();

            var admin = await _authService.GetMyProfile(
                int.Parse(userId)
            );

            if (admin == null)
                return NotFound(new { message = "Admin not found" });

            return Ok(admin);
        }


    }
}
