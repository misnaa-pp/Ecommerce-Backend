
using KidsWearAPI.Data;
using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using KidsWearAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace KidsWearAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        
        public async Task<string> Register(RegisterRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            
            var existingUser = await _context.Users
                .AnyAsync(u => u.Email == email);

            if (existingUser)
            {
                return "Email already registered";
            }

            
            var user = new User
            {
                Name = request.Name.Trim(),
                Email = email,
                Phone = request.Phone
            };

            
            user.PasswordHash =
                _passwordHasher.HashPassword(user, request.Password);

            
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return "Registration successful";
        }



        public async Task<AuthResponse?> Login(LoginRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.IsBlocked)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            
            string accessToken = GenerateJwtToken(user);

            
            string refreshToken = GenerateRefreshToken();

           
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = HashRefreshToken(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshTokenEntity);

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }



        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()
                ),

                new Claim(ClaimTypes.Name, user.Name),

                new Claim(ClaimTypes.Email, user.Email),

               new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(
                        jwtSettings["DurationInMinutes"]
                    )
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }


        private string GenerateRefreshToken()
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }

        private string HashRefreshToken(string token)
        {
            byte[] hash = SHA256.HashData(
                Encoding.UTF8.GetBytes(token)
            );

            return Convert.ToHexString(hash);
        }



        public async Task<AuthResponse?> RefreshAccessToken(
            RefreshTokenRequest request)
        {
            
            string tokenHash = HashRefreshToken(request.RefreshToken);

            
            var oldToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

            if (oldToken == null)
            {
                return null;
            }

            if (oldToken.User.IsBlocked)
            {
                var activeTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == oldToken.UserId
                        && rt.RevokedAt == null)
                    .ToListAsync();

                foreach (var token in activeTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return null;
            }


            if (oldToken.RevokedAt != null)
            {
              
                var activeTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == oldToken.UserId
                        && rt.RevokedAt == null)
                    .ToListAsync();

                foreach (var token in activeTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return null;
            }

            
            if (oldToken.ExpiresAt <= DateTime.UtcNow)
            {
                return null;
            }

            
            string newAccessToken = GenerateJwtToken(oldToken.User);
            string newRefreshToken = GenerateRefreshToken();

            string newTokenHash = HashRefreshToken(newRefreshToken);

            
            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.ReplacedByTokenHash = newTokenHash;

           
            var newToken = new RefreshToken
            {
                UserId = oldToken.UserId,
                TokenHash = newTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(newToken);

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }



        public async Task<object?> GetMyProfile(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    u.UserId,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.Address,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            return user;
        }


        public async Task<string?> UpdateMyProfile(
            int userId,
            UpdateProfileRequest request)
        {
           
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return "User not found";
            }

          
            string email = request.Email.Trim().ToLowerInvariant();

            bool emailExists = await _context.Users
                .AnyAsync(u =>
                    u.Email.ToLower() == email &&
                    u.UserId != userId);

            if (emailExists)
            {
                return "Email already registered";
            }

            user.Name = request.Name.Trim();
            user.Email = email;
            user.Phone = request.Phone?.Trim();
            user.Address = request.Address?.Trim();

            await _context.SaveChangesAsync();

            return null;
        }



        public async Task<bool> Logout(RefreshTokenRequest request)
        {
            string tokenHash = HashRefreshToken(request.RefreshToken);

            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

            if (refreshToken == null || refreshToken.RevokedAt != null)
            {
                return false;
            }

            refreshToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}

