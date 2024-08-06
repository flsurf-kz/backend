using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class UpdateChat : BaseUseCase<InputDTO, OutputDTO>
    {
        public UpdateChat() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
