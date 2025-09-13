using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductModule.Application.Characteristic.Queries.GetProductCharacterisitcs;
using ProductModule.Application.Dtos;
using ProductModule.Application.Product.Commands.CreateProduct;
using ProductModule.Application.Product.Commands.DeleteProduct;
using ProductModule.Application.Product.Commands.PublishProduct;
using ProductModule.Application.Product.Commands.UpdateProduct;
using ProductModule.Application.Product.Queries.GetFilteredProducts;
using ProductModule.Application.Product.Queries.GetMyProducts;
using ProductModule.Application.Product.Queries.GetProduct;
using ProductModule.Application.Product.Queries.GetProductByCategory;
using ProductModule.Application.Product.Queries.GetUserProducts;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetMyProducts([FromQuery] GetMyProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await _mediator.Send(query, cancellationToken);
            return Ok(products);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetUserProducts([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var query = new GetUserProductsQuery(userId);
            var products = await _mediator.Send(query, cancellationToken);
            return Ok(products);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategoryId([FromRoute] int categoryId, CancellationToken cancellationToken)
        {
            var query = new GetProductsByCategoryQuery { CategoryId = categoryId };
            var products = await _mediator.Send(query, cancellationToken);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetProductQuery(id);
            var product = await _mediator.Send(query, cancellationToken);

            if (product == null)
                return NotFound("Product not found.");

            return Ok(product);
        }

        [HttpGet("{id}/characteristics")]
        public async Task<ActionResult<IEnumerable<CharacteristicGroupDto>>> GetProductCharacteristics([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetProductCharacteristicsQuery(id), cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByFilter([FromQuery] GetFilteredProductQuery query, CancellationToken cancellationToken)
        {
            var products = await _mediator.Send(query, cancellationToken);
            return Ok(products);
        }

        [Authorize]
        [HttpPatch("{productId}/publish")]
        public async Task<ActionResult> PublishProduct([FromRoute] Guid productId, CancellationToken cancellationToken)
        {
            var command = new PublishProductCommand { ProductId = productId };
            await _mediator.Send(command, cancellationToken);
            return Ok("Product published successfully.");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> AddProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            var productId = await _mediator.Send(command, cancellationToken);
            return Ok(productId);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok("Product updated successfully.");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand(id);
            await _mediator.Send(command, cancellationToken);
            return Ok("Product deleted successfully.");
        }
    }
}
