// ================================================================
// ContractSentToFreelancer -> уведомляем фрилансера
// ================================================================
using Flsurf.Application.Common.Events;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.User.Commands;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flsurf.Application.Freelance.EventHandlers;

public sealed class ContractSentToFreelancerNotificationHandler(
        ILogger<ContractSentToFreelancerNotificationHandler> log,
        IApplicationDbContext ctx,
        IPermissionService perm)
    : IIntegrationEventSubscriber<ContractSentToFreelancer>
{
    public async Task HandleEvent(ContractSentToFreelancer evt)
    {
        var title = await ctx.Contracts
                             .AsNoTracking()
                             .Include(c => c.Job)
                             .Where(c => c.Id == evt.ContractId)
                             .Select(c => c.Job.Title)
                             .FirstAsync();

        await new CreateNotificationHandler(ctx, perm).Handle(
            new CreateNotificationCommand
            {
                Title  = "Новый контракт",
                Text   = $"Вам отправлен контракт «{title}». Ознакомьтесь и подпишите.",
                UserId = evt.FreelancerId,
                Data   = new() { ["contractId"] = evt.ContractId.ToString() }
            });

        log.LogInformation("Notification sent to freelancer {F} about contract {C}",
                           evt.FreelancerId, evt.ContractId);
    }
}


public sealed class ContractWasCreatedNotificationHandler(
        ILogger<ContractWasCreatedNotificationHandler> log,
        IApplicationDbContext ctx,
        IPermissionService perm)
    : IIntegrationEventSubscriber<ContractWasCreated>
{
    public async Task HandleEvent(ContractWasCreated evt)
    {
        var contract = await ctx.Contracts
                                .AsNoTracking()
                                .Include(c => c.Job)
                                .FirstAsync(c => c.Id == evt.ContractId);

        var cmdBase = new CreateNotificationCommand
        {
            Title = "Контракт подписан",
            Text  = $"Контракт «{contract.Job.Title}» вступил в силу.",
            Data  = new() { ["contractId"] = contract.Id.ToString() }
        };

        foreach (var uid in new[] { contract.EmployerId, contract.FreelancerId })
        {
            cmdBase.UserId = uid;
            await new CreateNotificationHandler(ctx, perm).Handle(cmdBase);
        }

        log.LogInformation("Notified employer {E} and freelancer {F} about contract {C}",
                           contract.EmployerId, contract.FreelancerId, evt.ContractId);
    }
}

public sealed class ReactedToProposalNotificationHandler(
        ILogger<ReactedToProposalNotificationHandler> log,
        IApplicationDbContext ctx,
        IPermissionService perm)
    : IIntegrationEventSubscriber<ReactedToProposal>
{
    public async Task HandleEvent(ReactedToProposal evt)
    {
        var proposal = await ctx.Proposals
                                .AsNoTracking()
                                .Include(p => p.Job)
                                .FirstAsync(p => p.Id == evt.ProposalId);

        await new CreateNotificationHandler(ctx, perm).Handle(
            new CreateNotificationCommand
            {
                Title  = "Новая ставка",
                Text   = $"Фрилансер откликнулся на ваш проект «{proposal.Job.Title}».",
                UserId = proposal.Job.EmployerId,
                Data   = new()
                {
                    ["jobId"]      = proposal.JobId.ToString(),
                    ["proposalId"] = proposal.Id.ToString()
                }
            });

        log.LogInformation("Notify client {C} about proposal {P}",
                           proposal.Job.EmployerId, evt.ProposalId);
    }
}


public sealed class FreelancerFinishedContractNotificationHandler(
        ILogger<FreelancerFinishedContractNotificationHandler> log,
        IApplicationDbContext ctx,
        IPermissionService perm)
    : IIntegrationEventSubscriber<FreelancerFinishedContract>
{
    public async Task HandleEvent(FreelancerFinishedContract evt)
    {
        var contract = await ctx.Contracts
                                .AsNoTracking()
                                .Include(c => c.Job)
                                .FirstAsync(c => c.Id == evt.ContractId);

        await new CreateNotificationHandler(ctx, perm).Handle(
            new CreateNotificationCommand
            {
                Title  = "Работа завершена",
                Text   = $"Фрилансер сообщил о завершении контракта «{contract.Job.Title}». " +
                         $"Проверьте и подтвердите результат.",
                UserId = contract.EmployerId,
                Data   = new() { ["contractId"] = contract.Id.ToString() }
            });

        log.LogInformation("Notify client {C} that freelancer finished contract {Id}",
                           contract.EmployerId, evt.ContractId);
    }
}

public sealed class ContractFinishedNotificationHandler(
        ILogger<ContractFinishedNotificationHandler> log,
        IApplicationDbContext ctx,
        IPermissionService perm)
    : IIntegrationEventSubscriber<ContractFinished>
{
    public async Task HandleEvent(ContractFinished evt)
    {
        var contract = await ctx.Contracts
                                .AsNoTracking()
                                .Include(c => c.Job)
                                .FirstAsync(c => c.Id == evt.ContractId);

        var cmdBase = new CreateNotificationCommand
        {
            Title = "Контракт завершён",
            Text  = $"Контракт «{contract.Job.Title}» закрыт. Спасибо за сотрудничество!",
            Data  = new() { ["contractId"] = contract.Id.ToString() }
        };

        foreach (var uid in new[] { contract.EmployerId, contract.FreelancerId })
        {
            cmdBase.UserId = uid;
            await new CreateNotificationHandler(ctx, perm).Handle(cmdBase);
        }

        log.LogInformation("Notify parties contract {Id} finished", evt.ContractId);
    }
}
