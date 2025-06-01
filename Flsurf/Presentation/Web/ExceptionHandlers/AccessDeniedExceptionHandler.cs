using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Flsurf.Presentation.Web.ExceptionHandlers
{
    public class AccessDeniedExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<AccessDeniedExceptionFilter> _logger;

        public AccessDeniedExceptionFilter(ILogger<AccessDeniedExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            // если экс — AccessDenied, формируем ответ
            if (context.Exception is AccessDenied ex)
            {
                _logger.LogWarning(ex, "Доступ запрещён: {Message}", ex.Message);

                var problem = new
                {
                    status = StatusCodes.Status403Forbidden,
                    title = "Access Denied",
                    detail = ex.Message
                };

                context.Result = new JsonResult(problem)
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    ContentType = "application/problem+json"
                };

                context.ExceptionHandled = true; // больше никто не будет его обрабатывать
            }
        }
    }
}
