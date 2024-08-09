using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetMessagesList : BaseUseCase<GetMessagesListDto, ICollection<MessageEntity>>
    {
        public GetMessagesList() { }

        public async Task<ICollection<MessageEntity>> Execute(GetMessagesListDto dto)
        {
            return [];
        }
    }
}
