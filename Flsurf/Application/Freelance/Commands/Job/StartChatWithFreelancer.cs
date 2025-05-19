using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Messaging.Interfaces;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public class StartChatWithFreelancerCommand : BaseCommand
    {
        public Guid ProposalId { get; set; }
        public Guid JobId { get; set; }
    }

    public class StartChatWithFreelancerHandler(IApplicationDbContext dbContext, IPermissionService permService, IChatService chatService) : ICommandHandler<StartChatWithFreelancerCommand>
    {
        public async Task<CommandResult> Handle(StartChatWithFreelancerCommand cmd)
        {
            var job = await dbContext.Jobs
                .Include(x => x.Proposals)
                .FirstOrDefaultAsync(x => x.Id == cmd.JobId);
            var currentUser = await permService.GetCurrentUser();

            if (job == null)
                return CommandResult.NotFound("Работа не найдена", cmd.JobId);
            if (job.Status != Domain.Freelance.Enums.JobStatus.Open)
                return CommandResult.Conflict("Работа не открыта"); 

            if (currentUser.Type != Domain.User.Enums.UserTypes.Client || currentUser.Id != job.EmployerId)
                return CommandResult.Forbidden("Не клиент или не ваша работа");

            var proposal = await dbContext.Proposals.FirstOrDefaultAsync(x => x.Id == cmd.ProposalId);
            if (proposal == null)
                return CommandResult.NotFound("Ставка не найдена", cmd.ProposalId);
            if (!job.Proposals.Contains(proposal))
                return CommandResult.NotFound("Ставка не найдена в этой работе", cmd.ProposalId); 
            
            var freelancerId = proposal.FreelancerId;

            var chat = await dbContext.Chats.FirstOrDefaultAsync(
                x => x.Participants.Select(x => x.Id).Contains(freelancerId) || x.OwnerId == freelancerId && !x.IsArchived);

            if (chat != null)
            {
                if (chat.IsArchived)
                    chat.IsArchived = false;
                chat.Jobs.Add(job);
                job.Chats.Add(chat);

                await dbContext.SaveChangesAsync(); 

                return CommandResult.Success(chat.Id);
            }

            var newChatId = await chatService.Create().Execute(
                new Messaging.Dto.CreateChatDto()
                {
                    Name = $"{job.Title}", 
                    Description = $"Чат связан с заказом: {job.Title}", 
                    UserIds = [freelancerId], 
                    type = Domain.Messanging.Enums.ChatTypes.Direct, 
                });
            var newChat = await dbContext.Chats.FirstOrDefaultAsync(x => x.Id == newChatId);

            if (newChat == null)
                throw new Exception("Что то пошло не так с СУБД");

            newChat.Jobs.Add(job); 
            job.Chats.Add(newChat);

            await dbContext.SaveChangesAsync();

            return CommandResult.Success(newChatId); 
        } 
    }
}
