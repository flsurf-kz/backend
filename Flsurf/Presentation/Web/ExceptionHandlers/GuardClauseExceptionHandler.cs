using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Flsurf.Presentation.Web.ExceptionHandlers
{

    public class GuardClauseExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<GuardClauseExceptionFilter> _logger;

        public GuardClauseExceptionFilter(ILogger<GuardClauseExceptionFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void OnException(ExceptionContext context)
        {
            // Обработка нарушения guard clause (ArgumentNullException)
            if (context.Exception is ArgumentNullException argNullEx)
            {
                _logger.LogError(argNullEx, "Guard clause violation: {Message}", argNullEx.Message);
                context.Result = new JsonResult(new { ErrorMessage = argNullEx.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
                context.ExceptionHandled = true;
                return;
            }

            // Обработка исключений, относящихся к пространству имён "Flsurf"
            var ns = context.Exception.TargetSite?.DeclaringType?.Namespace;
            if (!string.IsNullOrEmpty(ns) && ns.StartsWith("Flsurf"))
            {
                _logger.LogError(context.Exception, "Exception in Flsurf namespace: {Message}", context.Exception.Message);
                context.Result = new JsonResult(new { ErrorMessage = "An error occurred within your namespace." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                context.ExceptionHandled = true;
                return;
            }

            // Если исключение не соответствует заданным условиям, оставляем его для дальнейшей обработки
            base.OnException(context);
        }
    }
}
