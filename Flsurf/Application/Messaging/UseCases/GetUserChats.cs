using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetUserChats : BaseUseCase<InputDTO, OutputDTO>
    {
        public GetUserChats() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
