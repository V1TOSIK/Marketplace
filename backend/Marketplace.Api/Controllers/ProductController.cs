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
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] AddProductRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!Guid.TryParse(userId, out var parsedUserId))
                return BadRequest("Invalid user ID.");

            await _productService.AddProductAsync(parsedUserId, request);

            return Ok("Product added successfully.");
        }
    }
}
