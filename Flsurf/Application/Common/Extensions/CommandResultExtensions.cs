using Flsurf.Application.Common.cqrs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Flsurf.Application.Common.Extensions
{
    public static class CommandResultExtensions
    {
        public static ActionResult<T> MapResult<T>(this CommandResult result, ControllerBase controller)
        {
            return result.Status switch
            {
                HttpStatusCode.OK => controller.Ok(
                    result.Id is null
                        ? (T)Convert.ChangeType(result.Ids, typeof(T))
                        : (T)Convert.ChangeType(result.Id, typeof(T))),
                HttpStatusCode.BadRequest => controller.BadRequest(result.Message),
                HttpStatusCode.NotFound => controller.NotFound(result.Message),
                HttpStatusCode.Forbidden => controller.StatusCode((int)HttpStatusCode.Forbidden, result.Message),
                HttpStatusCode.UnprocessableEntity => controller.UnprocessableEntity(result.Message),
                HttpStatusCode.InternalServerError => controller.StatusCode((int)HttpStatusCode.InternalServerError, result.Message),
                HttpStatusCode.NoContent => controller.NoContent(),
                HttpStatusCode.Conflict => controller.Conflict(result.Message),
                _ => controller.StatusCode((int)result.Status, result.Message),
            };
        }
    }
}
