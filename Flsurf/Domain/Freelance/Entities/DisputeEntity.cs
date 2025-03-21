using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class DisputeEntity : BaseAuditableEntity
    {
        [ForeignKey(nameof(Contract))]
        public Guid ContractId { get; set; }
        public ContractEntity Contract { get; set; } = null!;

        // Инициатор спора
        [ForeignKey(nameof(Initiator))]
        public Guid InitiatorId { get; set; }
        public UserEntity Initiator { get; set; } = null!;

        // Причина спора
        public string Reason { get; set; } = string.Empty;

        // Статус спора
        public DisputeStatus Status { get; set; } = DisputeStatus.Pending;

        // Ссылка на тикет в Staff-контексте (AMC)
        public Guid? StaffTicketId { get; set; }

        // Ссылка на чат в Messenger-контексте
        public Guid? MessengerChatId { get; set; }

        // История изменения статусов спора (для аудита)
        public ICollection<DisputeStatusHistory> StatusHistory { get; set; } = [];

        // Комментарий модератора при решении спора
        public string? ModeratorComment { get; set; }

        // Удобный метод для смены статуса
        public void ChangeStatus(DisputeStatus newStatus, string comment = "")
        {
            Status = newStatus;
            StatusHistory.Add(new DisputeStatusHistory
            {
                DisputeId = this.Id,
                Status = newStatus,
                Comment = comment,
                ChangedAt = DateTime.UtcNow
            });

            AddDomainEvent(new DisputeStatusChangedEvent(this.Id, newStatus));
        }
    }

}
