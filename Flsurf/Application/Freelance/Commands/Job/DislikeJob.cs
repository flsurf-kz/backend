using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public sealed class DislikeJobCommand : BaseCommand
    {
        public required Guid JobId { get; init; }
    }

    public sealed class DislikeJobHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService,
        IDistributedCache cache)
    : ICommandHandler<DislikeJobCommand>
    {
        private string CacheKey(Guid jobId) => $"job:{jobId}:dislikes";

        public async Task<CommandResult> Handle(DislikeJobCommand cmd)
        {
            /* 1. Аутентификация */
            var user = await permService.GetCurrentUser();
            if (user == null)                   // на всякий
                return CommandResult.Forbidden("Not authenticated");

            /* 2. Существование Job */
            var job = await dbContext.Jobs
                .FirstOrDefaultAsync(j => j.Id == cmd.JobId);

            if (job is null)
                return CommandResult.NotFound("Вакансия не найдена", cmd.JobId);

            /* 3. Достаём/создаём список дизлайкнувших */
            var key = CacheKey(cmd.JobId);
            var bytes = await cache.GetAsync(key);
            HashSet<Guid> users = bytes is null
                ? new() : JsonSerializer.Deserialize<HashSet<Guid>>(bytes)!;

            /* 4. Если уже дизлайкнул — просто вернуть текущее значение */
            if (!users.Add(user.Id))
                return CommandResult.Success(job.Id);

            /* 5. Сохраняем обратно в DistributedCache (24 ч sliding) */
            var opts = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(24)
            };
            await cache.SetAsync(key,
                JsonSerializer.SerializeToUtf8Bytes(users), opts);

            /* 6. Пересчитываем и обновляем Domain-модель */
            job.ApplyDislikes(users.Count);
            await dbContext.SaveChangesAsync();

            return CommandResult.Success($"{job.DislikesCount}", job.Id);
        }
    }
}
