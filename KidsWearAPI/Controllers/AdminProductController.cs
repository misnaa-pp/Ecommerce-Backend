

using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsWearAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminProductController : ControllerBase
    {
        private readonly IAdminProductService _productService;

        public AdminProductController(IAdminProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? category = null)
        {
            var result = await _productService.GetAllProducts(
                pageNumber,
                pageSize,
                search,
                category
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);

            if (product == null)
                return NotFound(new { message = "Product not found." });

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(
            [FromBody] ProductRequest request)
        {
            var product = await _productService.AddProduct(request);

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.ProductId },
                product
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            [FromBody] ProductRequest request)
        {
            var product = await _productService.UpdateProduct(id, request);

            if (product == null)
                return NotFound(new { message = "Product not found." });

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);

            if (result == "NotFound")
                return NotFound(new { message = "Product not found." });

            if (result == "UsedInOrder")
                return BadRequest(new
                {
                    message = "This product cannot be deleted because it is already included in an order."
                });
            if (result == "InCart")
                return BadRequest("Cannot delete this product because it exists in customer carts.");

            if (result == "InWishlist")
                return BadRequest("Cannot delete this product because it exists in customer wishlists.");

            return Ok(new { message = "Product deleted successfully." });
        }
    }
}

