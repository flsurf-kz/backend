using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Common
{
    public class BaseEntity
    {
        [Key, Required, JsonPropertyOrder(100)]
        public Guid Id { get; set; }

        [Newtonsoft.Json.JsonIgnore, NotMapped]
        private readonly List<BaseEvent> _domainEvents = new();

        [NotMapped, Newtonsoft.Json.JsonIgnore]
        public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        public void AddDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
