using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.UseCases;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class AddWorkSnapshotCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
        public DateTime Timestamp { get; set; }
        public CreateFileDto File { get; set; } = null!;
        public string? Comment { get; set; }
    }


    public class AddWorkSnapshotHandler : ICommandHandler<AddWorkSnapshotCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;
        private readonly UploadFiles _uploadFiles;

        public AddWorkSnapshotHandler(IApplicationDbContext context, IPermissionService permService, UploadFiles uploadFiles)
        {
            _context = context;
            _permService = permService;
            _uploadFiles = uploadFiles;
        }

        public async Task<CommandResult> Handle(AddWorkSnapshotCommand command)
        {
            var user = await _permService.GetCurrentUser();
            var contract = await _context.Contracts.IncludeStandard()
                .FirstOrDefaultAsync(c => c.Id == command.ContractId);

            if (contract == null)
                return CommandResult.NotFound("Contract not found", command.ContractId);

            if (!await _permService.CheckPermission(ZedFreelancerUser.WithId(user.Id).CanWorkOnContract(command.ContractId)))
                return CommandResult.Forbidden("You do not have permission to add a snapshot to this contract");

            var uploadedFile = await _uploadFiles.Execute([command.File]);

            var snapshot = WorkSnapshotEntity.Create(contract, user, uploadedFile.First(), command.Timestamp, command.Comment);

            _context.WorkSnapshots.Add(snapshot);
            await _context.SaveChangesAsync();

            return CommandResult.Success(snapshot.Id);
        }
    }

}
