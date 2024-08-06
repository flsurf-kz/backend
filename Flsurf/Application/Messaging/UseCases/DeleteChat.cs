using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class DeleteChat : BaseUseCase<InputDTO, OutputDTO>
    {
        public DeleteChat() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
