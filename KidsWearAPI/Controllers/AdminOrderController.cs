
using KidsWearAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KidsWearAPI.DTOs;

namespace KidsWearAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : ControllerBase
    {
        private readonly IAdminOrderService _orderService;

        public AdminOrderController(IAdminOrderService orderService)
        {
            _orderService = orderService;
        }



        [HttpGet]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Invalid pagination parameters.");
            }

            var orders = await _orderService.GetFilteredOrders(
                pageNumber,
                pageSize,
                search,
                status
            );

            return Ok(orders);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);

            if (order == null)
                return NotFound(new { message = "Order not found" });

            return Ok(order);
        }


        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var order = await _orderService.GetOrderDetails(id);

            if (order == null)
                return NotFound(new { message = "Order not found" });

            return Ok(order);
        }




        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int id,
            [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _orderService.UpdateOrderStatus(
                id,
                request.Status
            );

            if (result == "NotFound")
            {
                return NotFound(new
                {
                    message = "Order not found"
                });
            }

            if (result == "InvalidTransition")
            {
                return BadRequest(new
                {
                    message = "Invalid order status transition"
                });
            }

            if (result == "RefundRequired")
            {
                return BadRequest(
                    "This order has a successful online payment. Refund handling is required before cancellation."
                );
            }

            return Ok(new
            {
                message = "Order status updated successfully"
            });
        }


    }
}
