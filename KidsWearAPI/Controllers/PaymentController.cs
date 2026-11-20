
using KidsWearAPI.DTOs.Payment;
using KidsWearAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KidsWearAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateRazorpayOrder(
            [FromQuery] int orderId)
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            try
            {
                var result = await _paymentService.CreateRazorpayOrder(
                    orderId, userId);

                if (result == null)
                    return NotFound(new
                    {
                        message = "Order not found."
                    });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyRazorpayPayment(
            [FromBody] VerifyRazorpayPaymentRequest request)
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            try
            {
                var result = await _paymentService.VerifyRazorpayPayment(
                    request, userId);

                if (result == null)
                    return NotFound(new
                    {
                        message = "Order not found."
                    });

                return Ok(new
                {
                    message = "Payment verified successfully.",
                    payment = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
