using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetUserChats : BaseUseCase<GetUserChatsDto, ICollection<UserToChatEntity>>
    {
        public GetUserChats() { }

        public async Task<ICollection<UserToChatEntity>> Execute(GetUserChatsDto dto)
        {
            return [];
        }
    }
}
