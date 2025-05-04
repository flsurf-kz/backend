using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetUnreadCounter(IApplicationDbContext dbContext) : BaseUseCase<Guid?, int> 
    {
        public async Task<int> Execute(Guid? chatId)
        {
            // TODO
            return 1; 
        }
    }
}
