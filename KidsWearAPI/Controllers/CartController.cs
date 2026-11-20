
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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToCart(
            [Range(1, int.MaxValue)] int productId,
            [FromQuery, Required] string size,
            [FromQuery, Range(1, int.MaxValue)] int quantity = 1)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var result = await _cartService.AddToCart(
                userId, productId, size, quantity);

            if (!result)
                return BadRequest(
                    "Invalid product, size, quantity or insufficient stock.");

            return Ok("Product added to cart.");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var cart = await _cartService.GetCart(userId);

            return Ok(cart);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateQuantity(
            [Range(1, int.MaxValue)] int productId,
            [FromQuery, Required] string size,
            [FromQuery, Range(1, int.MaxValue)] int quantity)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var result = await _cartService.UpdateQuantity(
                userId, productId, size, quantity);

            if (!result)
                return BadRequest(
                    "Cart item not found or invalid quantity/stock.");

            return Ok("Cart quantity updated.");
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(
            [Range(1, int.MaxValue)] int productId,
            [FromQuery, Required] string size)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var result = await _cartService.RemoveFromCart(
                userId, productId, size);

            if (!result)
                return NotFound("Cart item not found.");

            return Ok("Product removed from cart.");
        }
    }
}
