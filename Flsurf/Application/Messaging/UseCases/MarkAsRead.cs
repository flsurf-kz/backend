using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class MarkAsRead : BaseUseCase<InputDTO, OutputDTO>
    {
        public MarkAsRead() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
