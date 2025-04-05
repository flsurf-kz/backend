using Flsurf.Application.Common.cqrs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Flsurf.Application.Common.Extensions
{
    public static class CommandResultExtensions
    {
        public static ActionResult MapResult(this CommandResult result, ControllerBase controller)
        {
            return result.Status switch
            {
                HttpStatusCode.OK => controller.Ok(result), 
                HttpStatusCode.BadRequest => controller.BadRequest(result),
                HttpStatusCode.NotFound => controller.NotFound(result),
                HttpStatusCode.Forbidden => controller.StatusCode((int)HttpStatusCode.Forbidden, result),
                HttpStatusCode.UnprocessableEntity => controller.UnprocessableEntity(result),
                HttpStatusCode.InternalServerError => controller.StatusCode((int)HttpStatusCode.InternalServerError, result),
                HttpStatusCode.NoContent => controller.NoContent(),
                HttpStatusCode.Conflict => controller.Conflict(result),
                _ => controller.StatusCode((int)result.Status, result),
            };
        }
    }
}
