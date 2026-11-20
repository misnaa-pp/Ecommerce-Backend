
using KidsWearAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KidsWearAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] string? search,
            [FromQuery] string? category,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetAllProducts(
                search,
                category,
                pageNumber,
                pageSize
            );

            return Ok(products);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            return Ok(product);
        }
    }
}
