using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;

namespace Flsurf.Application.Messaging.UseCases
{
    public class DeleteMessage : BaseUseCase<DeleteMessageDto, bool>
    {
        public DeleteMessage() { }

        public async Task<bool> Execute(DeleteMessageDto dto)
        {
            return true;
        }
    }
}
