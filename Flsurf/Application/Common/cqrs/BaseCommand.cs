using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Application.Common.cqrs
{
    public abstract class BaseCommand
    {
        [JsonIgnore, NotMapped]
        public string CommandId { get; } // Уникальный идентификатор команды

        [JsonIgnore, NotMapped]
        public DateTime Timestamp { get; } // Временная метка создания команды

        protected BaseCommand()
        {
            CommandId = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
        }

        protected BaseCommand(string commandId, DateTime? timestamp)
        {
            CommandId = !string.IsNullOrEmpty(commandId) ? commandId : Guid.NewGuid().ToString();
            Timestamp = timestamp ?? DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"BaseCommand(CommandId={CommandId}, Timestamp={Timestamp})";
        }
    }
}
