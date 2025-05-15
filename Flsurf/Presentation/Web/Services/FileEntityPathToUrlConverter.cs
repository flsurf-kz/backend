namespace Flsurf.Presentation.Web.Services
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using System;

    public class FileEntityJsonConverter
        : JsonConverter<Flsurf.Domain.Files.Entities.FileEntity>
    {
        private readonly IHttpContextAccessor _httpCtx;
        private readonly IWebHostEnvironment _env;

        public FileEntityJsonConverter(
            IHttpContextAccessor httpCtx,
            IWebHostEnvironment env)
        {
            _httpCtx = httpCtx;
            _env = env;
        }

        public override bool CanRead => false;

        public override void WriteJson(
            JsonWriter writer,
            Flsurf.Domain.Files.Entities.FileEntity? value,
            JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var http = _httpCtx.HttpContext!;
            var req = http.Request;
            var pathBase = req.PathBase.HasValue
                           ? req.PathBase.Value
                           : "";

            // Экранируем filePath
            var escaped = Uri.EscapeDataString(value.FilePath);

            string url;
            if (_env.IsDevelopment())
            {
                // локально — полный URL
                url = $"{req.Scheme}://{req.Host}{pathBase}/api/download/{escaped}";
            }
            else
            {
                // в проде (nginx) — относительный путь
                url = $"{pathBase}/api/download/{escaped}";
            }

            writer.WriteStartObject();

            writer.WritePropertyName("id");
            writer.WriteValue(value.Id);

            writer.WritePropertyName("fileName");
            writer.WriteValue(value.FileName);

            writer.WritePropertyName("filePath");
            writer.WriteValue(url);

            writer.WritePropertyName("mimeType");
            writer.WriteValue(value.MimeType);

            writer.WritePropertyName("size");
            writer.WriteValue(value.Size);

            writer.WriteEndObject();
        }

        public override Flsurf.Domain.Files.Entities.FileEntity ReadJson(
            JsonReader reader,
            Type objectType,
            Flsurf.Domain.Files.Entities.FileEntity? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
            => throw new NotSupportedException();
    }
}
