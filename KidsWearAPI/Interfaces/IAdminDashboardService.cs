

using KidsWearAPI.DTOs;

namespace KidsWearAPI.Interfaces
{
    public interface IAdminDashboardService
    {
        
        Task<AdminDashboardResponse> GetDashboard();

    }
}
