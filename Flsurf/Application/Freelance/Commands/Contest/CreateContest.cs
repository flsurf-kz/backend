using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Freelance.Commands.Contest
{
    public class CreateContestCommand : BaseCommand
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public decimal PrizePool { get; set; }
        public bool IsResultPublic { get; set; } = false; 
    }

    public class CreateContestHandler : ICommandHandler<CreateContestCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public CreateContestHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(CreateContestCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();

            if (currentUser.Type != Domain.User.Enums.UserTypes.Client)
            {
                return CommandResult.Forbidden("Не разрешено");
            }

            // Создание сущности конкурса
            var contest = new ContestEntity
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Description = command.Description,
                PrizePool = new Money(command.PrizePool),
                Status = ContestStatus.Draft,
                IsResultPublic = command.IsResultPublic,
                EmployerId = currentUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Contests.Add(contest);

            // Связь с ZedContract
            await _permService.AddRelationship(
                ZedContract.WithId(contest.Id).Client(ZedFreelancerUser.WithId(currentUser.Id)));

            await _context.SaveChangesAsync();

            return CommandResult.Success(contest.Id);
        }
    }
}
