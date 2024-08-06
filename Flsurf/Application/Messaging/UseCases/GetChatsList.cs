using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetChatsList : BaseUseCase<InputDTO, OutputDTO>
    {
        public GetChatsList() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
