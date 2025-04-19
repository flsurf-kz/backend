using Flsurf.Application.Common.cqrs;
using Flsurf.Domain.Freelance.Enums;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetContractsListQuery : BaseQuery
    {
        public Guid? UserId { get; set; } // Идентификатор пользователя
        public bool? IsClient { get; set; } // Если true – заказчик, если false – фрилансер
        public ContractStatus? Status { get; set; }
        public int Start { get; set; } = 0; // Пагинация
        public int Ends { get; set; } = 10; // Пагинация
    }
}
