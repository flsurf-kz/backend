using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Messaging.Interfaces;
using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public class StartChatWithFreelancerCommand : BaseCommand
    {
        public Guid ProposalId { get; set; }
        public Guid JobId { get; set; }
    }

    public class StartChatWithFreelancerHandler(
            IApplicationDbContext db,
            IPermissionService perm,
            IChatService chatSvc)
        : ICommandHandler<StartChatWithFreelancerCommand>
    {
        public async Task<CommandResult> Handle(StartChatWithFreelancerCommand cmd)
        {
            /* ── загрузка Job + проверки ───────────────────────────── */
            var job = await db.Jobs
                .Include(j => j.Proposals)
                .Include(j => j.Chats)          // ← понадобятся позже
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(j => j.Id == cmd.JobId);

            if (job is null)
                return CommandResult.NotFound("Работа не найдена", cmd.JobId);

            if (job.Status != Domain.Freelance.Enums.JobStatus.Open)
                return CommandResult.Conflict("Работа не открыта");

            var user = await perm.GetCurrentUser();
            if (user.Type != Domain.User.Enums.UserTypes.Client || user.Id != job.EmployerId)
                return CommandResult.Forbidden("Не клиент или не ваша работа");

            /* ── ставка ────────────────────────────────────────────── */
            var proposal = job.Proposals.FirstOrDefault(p => p.Id == cmd.ProposalId);
            if (proposal is null)
                return CommandResult.NotFound("Ставка не найдена в этой работе", cmd.ProposalId);

            Guid freelancerId = proposal.FreelancerId;

            /* ── ищем уже существующий чат клиент-фрилансер ────────── */

            ChatEntity? existingChat =
                // 1) среди чатов, уже связанных с Job
                job.Chats
                    .FirstOrDefault(c =>
                           (c.OwnerId == freelancerId || c.Participants.Any(p => p.Id == freelancerId))
                           && !c.IsArchived)
                // 2) либо любой другой Direct-чат
                ?? await db.Chats
                       .Include(c => c.Participants)
                       .FirstOrDefaultAsync(c =>
                           !c.IsArchived &&
                           c.Type == Domain.Messanging.Enums.ChatTypes.Direct &&
                          ((c.OwnerId == freelancerId && c.Participants.Any(p => p.Id == user.Id))
                           || (c.OwnerId == user.Id && c.Participants.Any(p => p.Id == freelancerId))));

            /* ───────────────────────────────────────────────────────── */

            if (existingChat is not null)
            {
                // вернуть связь Job-Chat, если её ещё нет
                if (!existingChat.Jobs.Any(j => j.Id == job.Id))
                {
                    existingChat.Jobs.Add(job);
                    job.Chats.Add(existingChat);
                    await db.SaveChangesAsync();
                }

                // Главное требование — Success() без payload
                return CommandResult.Success();
            }

            /* ── чата нет → создаём новый ──────────────────────────── */

            Guid newChatId = await chatSvc.Create().Execute(new Messaging.Dto.CreateChatDto
            {
                Name = job.Title,
                Description = $"Чат связан с заказом: {job.Title}",
                UserIds = [freelancerId],
                type = Domain.Messanging.Enums.ChatTypes.Direct
            });

            var newChat = await db.Chats.FirstAsync(c => c.Id == newChatId);

            newChat.Jobs.Add(job);
            job.Chats.Add(newChat);

            await db.SaveChangesAsync();

            return CommandResult.Success(newChatId);     // для фронта нужен id нового чата
        }
    }
}
