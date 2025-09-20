using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductModule.Application.Category.Commands.AddCategory;
using ProductModule.Application.Category.Commands.DeleteCategory;
using ProductModule.Application.Category.Commands.UpdateCategory;
using ProductModule.Application.Category.Queries.GetAllCategories;
using SharedKernel.Authorization.Attributes;
using SharedKernel.Authorization.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Marketplace.Api.Controllers
{
    [Route("api/Category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("all")]
        public async Task<ActionResult> GetAllCategories(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetAllCategoriesQuery(), cancellationToken));
        }

        [AuthorizeRole(nameof(AccessPolicy.Admin), nameof(AccessPolicy.Moderator))]
        [HttpPost]
        public async Task<ActionResult> AddCategory([FromBody] AddCategoryCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [AuthorizeRole(nameof(AccessPolicy.Admin), nameof(AccessPolicy.Moderator))]
        [HttpDelete("{categoryId}")]
        public async Task<ActionResult> DeleteCategory([FromRoute] int categoryId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteCategoryCommand(categoryId), cancellationToken);
            return Ok();
        }

        [AuthorizeRole(nameof(AccessPolicy.Admin), nameof(AccessPolicy.Moderator))]
        [HttpPut("{categoryId}")]
        public async Task<ActionResult> UpdateCategory([FromRoute] int categoryId, [FromBody] string newName, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateCategoryCommand(categoryId, newName), cancellationToken);
            return Ok();
        }
    }
}
