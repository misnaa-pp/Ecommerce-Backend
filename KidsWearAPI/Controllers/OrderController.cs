using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );
        }

        
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(
            CheckoutRequest request)
        {
            var order = await _orderService.Checkout(
                GetUserId(), request);

            if (order == null)
                return BadRequest(
                    "Invalid address, empty cart or insufficient stock.");

            return Ok(order);
        }

       
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders = await _orderService.GetMyOrders(GetUserId());

            return Ok(orders);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(
         [Range(1, int.MaxValue, ErrorMessage = "Invalid order ID")]
          int id)
        {
            var order = await _orderService.GetOrderById(
                GetUserId(), id);

            if (order == null)
                return NotFound("Order not found.");

            return Ok(order);
        }


        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(
         [Range(1, int.MaxValue, ErrorMessage = "Invalid order ID")]
         int id)
        {
            var result = await _orderService.CancelOrder(
                GetUserId(), id);

            if (!result)
                return BadRequest(
                    "Order not found or cannot be cancelled.");

            return Ok("Order cancelled successfully.");
        }
    }
}