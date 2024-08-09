using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;

namespace Flsurf.Application.Messaging.UseCases
{
    public class UpdateMessage : BaseUseCase<UpdateMessageDto, bool>
    {
        public UpdateMessage() { }

        public async Task<bool> Execute(UpdateMessageDto dto)
        {
            return true;
        }
    }
}
