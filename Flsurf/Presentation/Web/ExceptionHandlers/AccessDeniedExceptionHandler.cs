using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Flsurf.Presentation.Web.ExceptionHandlers
{
    public class AccessDeniedExceptionHandler : ExceptionFilterAttribute
    {
        private readonly ILogger<AccessDeniedExceptionHandler> _logger;

        public AccessDeniedExceptionHandler(ILogger<AccessDeniedExceptionHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ArgumentNullException argNullEx)
            {
                _logger.LogError(argNullEx, "Access denied: {Message}", argNullEx.Message);
                context.Result = new JsonResult(new { ErrorMessage = argNullEx.Message })
                {
                    StatusCode = StatusCodes.Status403Forbidden, 
                };
                context.ExceptionHandled = true;
                return;
            }


            base.OnException(context);
        }
    }
}
