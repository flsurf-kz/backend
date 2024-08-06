using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class DeleteMessage : BaseUseCase<InputDTO, OutputDTO>
    {
        public DeleteMessage() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
