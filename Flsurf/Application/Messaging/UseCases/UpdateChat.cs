using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;

namespace Flsurf.Application.Messaging.UseCases
{
    public class UpdateChat : BaseUseCase<UpdateChatDto, bool>
    {
        public UpdateChat() { }

        public async Task<bool> Execute(UpdateChatDto dto)
        {
            return true;
        }
    }
}
