using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Infrastructure.EventStore
{
    public class StoredEvent
    {
        [Key]
        public Guid EventId { get; set; }
        public Guid? ByUserId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [Required]
        public string EventType { get; set; } = null!;
        [Column(TypeName = "jsonb")]
        [Required]
        public string Data { get; set; } = null!;
        /// <summary>
        /// Указывает, является ли событие интеграционным (внешним).
        /// Для доменных или прочих событий ставим false.
        /// </summary>
        public bool IsIntegrationEvent { get; set; }

        /// <summary>
        /// Показывает, было ли событие уже обработано
        /// асинхронным воркером (true - да, false - нет).
        /// </summary>
        public bool Processed { get; set; } = false;
        public bool ProcessError { get; set; } = false;
        [Column(TypeName = "jsonb")]
        public string? ErrorData { get; set; } 
        public int FailedCounter { get; set; } = 0; 
    }
}
