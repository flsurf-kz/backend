using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Validations.Rules;
using Newtonsoft.Json;

namespace Flsurf.Application.Common.cqrs
{
    public abstract class BaseQuery
    {
        // Все эти поля нужны для составления очереди и разделение Запросов 
        [JsonIgnore, BindNever]
        public string QueryId { get; private set; } // Уникальный идентификатор запроса

        [JsonIgnore, BindNever]
        public DateTime Timestamp { get; private set; } // Временная метка создания запроса

        protected BaseQuery()
        {
            QueryId = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
        }

        protected BaseQuery(string queryId, DateTime? timestamp)
        {
            QueryId = !string.IsNullOrEmpty(queryId) ? queryId : Guid.NewGuid().ToString();
            Timestamp = timestamp ?? DateTime.UtcNow;
        }
    }
}
