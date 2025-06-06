﻿using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Flsurf.Application.Freelance.Commands.Category;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.AspNetCore.Authorization;

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

        [HttpPost("create", Name = "CreateCategory")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var handler = _categoryService.CreateCategory();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpPost("update", Name = "UpdateCategory")]
        [Authorize]
        public async Task<ActionResult<CommandResult>> UpdateCategory([FromBody] UpdateCategoryCommand command)
        {
            var handler = _categoryService.UpdateCategory();
            var result = await handler.Handle(command);
            return result.MapResult(this);
        }

        [HttpDelete("{categoryId}", Name = "DeleteCategory")]
        public async Task<ActionResult<CommandResult>> DeleteCategory(Guid categoryId)
        {
            var handler = _categoryService.DeleteCategory();
            var result = await handler.Handle(new DeleteCategoryCommand() { CategoryId = categoryId });
            return result.MapResult(this);
        }

        [HttpGet("list", Name = "GetCategories")]
        public async Task<ActionResult<ICollection<CategoryEntity>>> GetCategories([FromQuery] string? searchQuery)
        {
            var handler = _categoryService.GetCategories();
            // Для простых операций чтения можно возвращать сущности напрямую
            var categories = await handler.Handle(new GetCategoriesQuery() { SearchQuery = searchQuery });
            return Ok(categories);
        }

        [HttpGet("{categoryId}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryEntity?>> GetCategory(Guid categoryId)
        {
            var result = await _categoryService.GetCategory().Handle(new GetCategoryQuery() { CategoryId = categoryId });

            return result; 
        }
    }
}
