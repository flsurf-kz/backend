using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class UpdateMessage : BaseUseCase<InputDTO, OutputDTO>
    {
        public UpdateMessage() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
