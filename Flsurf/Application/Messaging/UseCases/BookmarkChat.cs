using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class BookmarkChat : BaseUseCase<InputDTO, OutputDTO>
    {
        public BookmarkChat() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
