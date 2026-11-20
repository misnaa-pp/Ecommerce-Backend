
using KidsWearAPI.DTOs;

namespace KidsWearAPI.Interfaces
{
    public interface IAdminUserService
    {
        Task<IEnumerable<AdminUserResponse>> GetAllUsers();


        Task<AdminUserResponse?> GetUserById(int id);

        Task<IEnumerable<AdminUserResponse>> SearchUsers(string? search);

        Task<bool> ToggleBlockUser(int id);

        Task<bool> IsUserBlocked(int id);

    }
}
