using ProductModule.Application.Characteristic.Queries.GetProductCharacterisitcs;
using ProductModule.Application.Product.Commands.CreateProduct;
using ProductModule.Application.Product.Commands.DeleteProduct;
using ProductModule.Application.Product.Commands.PublishProduct;
using ProductModule.Application.Product.Commands.UpdateProduct;
using ProductModule.Application.Product.Queries.GetFilteredProducts;
using ProductModule.Application.Product.Queries.GetMyProducts;
using ProductModule.Application.Product.Queries.GetProduct;
using ProductModule.Application.Product.Queries.GetProductByCategory;
using ProductModule.Application.Product.Queries.GetUserProducts;
using ProductModule.Application.Dtos;
using SharedKernel.Authorization.Attributes;
using SharedKernel.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

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

        [AuthorizeSameUser]
        [HttpGet("user/{userId}/my")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetMyProducts([FromRoute] Guid userId, [FromQuery] GetMyProductsRequest request, CancellationToken cancellationToken)
        {
            var products = await _mediator.Send(new GetMyProductsQuery(userId, request), cancellationToken);
            return Ok(products);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetUserProducts([FromRoute] Guid userId, [FromQuery] PaginationRequest request, CancellationToken cancellationToken)
        {
            var products = await _mediator.Send(new GetUserProductsQuery(userId, request), cancellationToken);
            return Ok(products);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategoryId([FromRoute] int categoryId, [FromQuery] PaginationRequest request, CancellationToken cancellationToken)
        {
            var products = await _mediator.Send(new GetProductsByCategoryQuery(categoryId, request), cancellationToken);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var product = await _mediator.Send(new GetProductQuery(id), cancellationToken);
            return Ok(product);
        }

        [HttpGet("{productId}/characteristics")]
        public async Task<ActionResult<IEnumerable<CharacteristicGroupDto>>> GetProductCharacteristics([FromRoute] Guid productId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetProductCharacteristicsQuery(productId), cancellationToken);
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
            await _mediator.Send(new PublishProductCommand(productId), cancellationToken);
            return Ok("Product published successfully.");
        }

        [AuthorizeSameUser]
        [HttpPost("user/{userId}")]
        public async Task<ActionResult<Guid>> AddProduct([FromRoute] Guid userId, [FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var productId = await _mediator.Send(new CreateProductCommand(userId, request), cancellationToken);
            return Ok(productId);
        }

        [Authorize]
        [HttpPut("{productId}")]
        public async Task<ActionResult> UpdateProduct([FromRoute] Guid productId, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateProductCommand(productId, request), cancellationToken);
            return Ok("Product updated successfully.");
        }

        [AuthorizeSameUser]
        [HttpDelete("user/{userId}/product/{productId}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] Guid userId, [FromRoute] Guid productId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteProductCommand(userId, productId), cancellationToken);
            return Ok("Product deleted successfully.");
        }
    }
}
