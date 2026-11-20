using KidsWearAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KidsWearAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }


        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            if (productId <= 0)
                return BadRequest("Product ID must be greater than zero.");

            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var result = await _wishlistService.AddToWishlist(userId, productId);

            if (!result)
                return BadRequest("Product does not exist or is already in wishlist.");

            return Ok("Product added to wishlist.");
        }


        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var wishlist = await _wishlistService.GetWishlist(userId);

            return Ok(wishlist);
        }



        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            if (productId <= 0)
                return BadRequest("Product ID must be greater than zero.");

            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var result = await _wishlistService.RemoveFromWishlist(
                userId, productId);

            if (!result)
                return NotFound("Wishlist item not found.");

            return Ok("Product removed from wishlist.");
        }

    }
}