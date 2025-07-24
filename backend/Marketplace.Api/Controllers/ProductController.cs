using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductModule.Application.Dtos.Requests;
using ProductModule.Application.Interfaces;
using System.Security.Claims;

namespace Marketplace.Api.Controllers
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

        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult> GetMyProducts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
                return BadRequest("Invalid user ID.");

            var products = await _productService.GetMyProducts(parsedUserId);
            return Ok(products);
        }

        [HttpGet("all")]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult> GetProductsByCategoryId([FromRoute] int categoryId)
        {
            var products = await _productService.GetProductsByCategoryIdAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductById([FromRoute] Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound("Product not found.");

            return Ok(product);
        }

        [HttpPost("filter")]
        public async Task<ActionResult> GetProductsByFilter([FromBody] ProductFilterRequest request)
        {
            var products = await _productService.GetProductsByFilterAsync(request);
            return Ok(products);
        }

        [Authorize]
        [HttpPost("publish/{productId}")]
        public async Task<ActionResult> PublishProduct(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var parsedUserId))
                return BadRequest("Invalid user ID.");

            await _productService.PublishProductAsync(productId, parsedUserId);
            return Ok("Product published successfully.");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] AddProductRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var parsedUserId))
                return BadRequest("Invalid user ID.");

            await _productService.AddProductAsync(parsedUserId, request);

            return Ok("Product added successfully.");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var parsedUserId))
                return BadRequest("Invalid user ID.");

            await _productService.DeleteProductAsync(id, parsedUserId);
            return Ok("Product deleted successfully.");
        }
    }
}
