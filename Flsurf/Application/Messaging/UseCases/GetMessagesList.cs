using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetMessagesList : BaseUseCase<InputDTO, OutputDTO>
    {
        public GetMessagesList() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
