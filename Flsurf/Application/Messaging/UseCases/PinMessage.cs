using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class PinMessage : BaseUseCase<InputDTO, OutputDTO>
    {
        public PinMessage() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
