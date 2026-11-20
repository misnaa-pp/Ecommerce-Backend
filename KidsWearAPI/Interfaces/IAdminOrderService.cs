
using KidsWearAPI.DTOs;

namespace KidsWearAPI.Interfaces
{
    public interface IAdminOrderService
    {
        Task<IEnumerable<AdminOrderResponse>> GetAllOrders();

        Task<AdminOrderResponse?> GetOrderById(int id);

        Task<PaginatedResponse<AdminOrderResponse>> GetFilteredOrders(int pageNumber,int pageSize, string? search,string? status);

        Task<AdminOrderDetailsResponse?> GetOrderDetails(int id);
        Task<string> UpdateOrderStatus(int id, string status);
    }
}
