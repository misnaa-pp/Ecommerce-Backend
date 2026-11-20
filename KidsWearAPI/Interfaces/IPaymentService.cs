using KidsWearAPI.DTOs.Payment;
using KidsWearAPI.Models;

namespace KidsWearAPI.Interfaces
{
    public interface IPaymentService
    {
        Task<CreateRazorpayOrderResponse?> CreateRazorpayOrder(
            int orderId,
            int userId
        );

        Task<Payment?> VerifyRazorpayPayment(
            VerifyRazorpayPaymentRequest request,
            int userId
        );
    }
}