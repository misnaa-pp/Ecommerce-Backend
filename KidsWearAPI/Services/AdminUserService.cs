
using KidsWearAPI.Data;
using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsWearAPI.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly AppDbContext _context;

        public AdminUserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminUserResponse>> GetAllUsers()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Role == "User")
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new AdminUserResponse
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    IsBlocked = u.IsBlocked
                })
                .ToListAsync();
        }

        public async Task<AdminUserResponse?> GetUserById(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.UserId == id && u.Role == "User")
                .Select(u => new AdminUserResponse
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    IsBlocked = u.IsBlocked
                })
                .FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<AdminUserResponse>> SearchUsers(string? search)
        {
            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.Role == "User")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(u =>
                    u.Name.Contains(search) ||
                    u.Email.Contains(search));
            }

            return await query
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new AdminUserResponse
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    IsBlocked = u.IsBlocked
                })
                .ToListAsync();
        }


        public async Task<bool> ToggleBlockUser(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id && u.Role == "User");

            if (user == null)
                return false;

            user.IsBlocked = !user.IsBlocked;

            if (user.IsBlocked)
            {
                var activeTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == id && rt.RevokedAt == null)
                    .ToListAsync();

                foreach (var token in activeTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsUserBlocked(int id)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == id && u.IsBlocked);
        }


    }
}
