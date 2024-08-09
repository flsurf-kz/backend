using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;

namespace Flsurf.Application.Messaging.UseCases
{
    public class PinMessage : BaseUseCase<PinMessageDto, bool>
    {
        public PinMessage() { }

        public async Task<bool> Execute(PinMessageDto dto)
        {
            return true;
        }
    }
}
