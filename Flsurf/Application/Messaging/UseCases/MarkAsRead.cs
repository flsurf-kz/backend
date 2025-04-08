using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;

namespace Flsurf.Application.Messaging.UseCases
{
    public class MarkAsRead : BaseUseCase<MarkAsReadDto, bool>
    {
        public MarkAsRead() { }

        public Task<bool> Execute(MarkAsReadDto dto)
        {
            return Task.FromResult(true);
        }
    }
}
