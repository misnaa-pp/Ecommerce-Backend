using KidsWearAPI.DTOs;

namespace KidsWearAPI.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse?> Checkout(
            int userId, CheckoutRequest request);

        Task<IEnumerable<OrderResponse>> GetMyOrders(int userId);

        Task<OrderResponse?> GetOrderById(int userId, int orderId);

        Task<bool> CancelOrder(int userId, int orderId);
    }
}