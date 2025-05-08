using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetChat(IApplicationDbContext dbContext) : BaseUseCase<Guid, ChatEntity?>
    {
        public async Task<ChatEntity?> Execute(Guid chatId)
        {
            var chat = await dbContext.Chats
                .IncludeStandard()
                .Include(c => c.Messages              // filtered include
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(1))
                .FirstOrDefaultAsync(x => x.Id == chatId);
            return chat; 
        } 
    }
}
