using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Flsurf.Application.Freelance.Commands.Category;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;

namespace Flsurf.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<CommandResult>> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var handler = _categoryService.CreateCategory();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update")]
        public async Task<ActionResult<CommandResult>> UpdateCategory([FromBody] UpdateCategoryCommand command)
        {
            var handler = _categoryService.UpdateCategory();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<CommandResult>> DeleteCategory([FromBody] DeleteCategoryCommand command)
        {
            var handler = _categoryService.DeleteCategory();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpGet("list")]
        public async Task<ActionResult<ICollection<CategoryEntity>>> GetCategories()
        {
            var handler = _categoryService.GetCategories();
            // Для простых операций чтения можно возвращать сущности напрямую
            var categories = await handler.Handle(new GetCategoriesQuery());
            return Ok(categories);
        }
    }
}
