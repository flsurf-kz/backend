using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Flsurf.Presentation.Web.Schemas;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Application.User.Interfaces;
using Flsurf.Application.User.Dto;

namespace Flsurf.Presentation.Web.Controllers
{
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

        [HttpPatch("{userId}")]
        public async Task<ActionResult<bool>> UpdateUser(Guid userId, [FromBody] UpdateUserDto model)
        {
            var result = await UserService
                .Update()
                .Execute(model);
            return true; 
        }

        [HttpPatch("me")]
        public async Task<ActionResult<bool>> UpdateMe([FromBody] UpdateUserDto model)
        {
            var result = await UserService
                .Update()
                .Execute(model);
            return true;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserEntity>> GetMe()
        {
            var result = await UserService
                .Get().Execute(new GetUserDto() { UserId = _user.Id });

            return result;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserEntity>> GetUserById(Guid userId)
        {
            var result = await UserService
                .Get().Execute(new GetUserDto() { UserId = userId });

            return result;
        }
    }
}
