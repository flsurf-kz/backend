using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Options;


namespace Flsurf.Presentation.Web.Services
{
    public class SwaggerFileUpdater : IHostedService
    {
        private readonly ISwaggerProvider _swaggerProvider;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SwaggerFileUpdater> _logger;
        private readonly IOptions<SwaggerOptions> _swaggerOptions;

        public SwaggerFileUpdater(
            ISwaggerProvider swaggerProvider,
            IWebHostEnvironment env,
            ILogger<SwaggerFileUpdater> logger,
            IOptions<SwaggerOptions> swaggerOptions)
        {
            _swaggerProvider = swaggerProvider;
            _env = env;
            _logger = logger;
            _swaggerOptions = swaggerOptions;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Обычно обновляем файл только в режиме разработки
            if (_env.IsDevelopment())
            {
                try
                {
                    // Получаем Swagger-документ, версия "v1" должна соответствовать вашей настройке
                    var swaggerDoc = _swaggerProvider.GetSwagger("v1");
                    // Сериализуем документ в JSON (можно использовать System.Text.Json или Newtonsoft.Json)
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(swaggerDoc, Newtonsoft.Json.Formatting.Indented);
                    // Определяем путь для сохранения файла, например, в корне проекта или в wwwroot
                    var filePath = Path.Combine(_env.ContentRootPath, "swagger.json");
                    File.WriteAllText(filePath, json);
                    _logger.LogInformation("Swagger JSON file updated at {FilePath}", filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update Swagger JSON file.");
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

}
