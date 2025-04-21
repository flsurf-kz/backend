using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Flsurf.Presentation.Web.Schemas;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Application.User.Interfaces;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Queries;
using Flsurf.Application.User.Commands;
using Flsurf.Application.Common.Extensions;
using Flsurf.Application.Common.cqrs;

namespace Flsurf.Presentation.Web.Controllers
{
    [Authorize]
    [SwaggerTag("auth")]
    [Route("api/user/")]
    [ApiController]
    public class UserControllers : ControllerBase
    {
        public IUserService UserService;
        private IUser _user;

        public UserControllers(IUserService userService, IUser user)
        {
            UserService = userService;
            _user = user;
        }

        [HttpPatch("{userId}", Name = "UpdateUser")]
        public async Task<ActionResult<bool>> UpdateUser(Guid userId, [FromBody] UpdateUserCommand model)
        {
            var result = await UserService
                .UpdateUser()
                .Handle(model);
            return result.MapResult(this);
        }

        [HttpPatch("me", Name = "UpdateMe")]
        public async Task<ActionResult<bool>> UpdateMe([FromBody] UpdateUserCommand model)
        {
            var result = await UserService
                .UpdateUser()
                .Handle(model);
            return result.MapResult(this);
        }

        [HttpGet("me", Name = "GetMe")]
        public async Task<ActionResult<UserEntity?>> GetMe()
        {
            var result = await UserService
                .GetUser().Handle(new GetUserQuery() { UserId = _user.Id });

            return result;
        }

        [HttpGet("{userId}", Name = "GetUserById")]
        public async Task<ActionResult<UserEntity?>> GetUserById(Guid userId)
        {
            var result = await UserService
                .GetUser().Handle(new GetUserQuery() { UserId = userId });

            return result;
        }

        [HttpPut("{userId}/taxinfo", Name = "UpdateTaxInfo")]
        public async Task<ActionResult<CommandResult>> UpdateTaxInfo(Guid userId, [FromBody] UpdateTaxSettingsCommand command)
        {
            var result = await UserService.UpdateTaxSettings()
                .Handle(command);

            return result.MapResult(this); 
        }
    }
}
