using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.Events;
using Flsurf.Application.Files.Permissions;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.FileStorage;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.EventDispatcher;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Files.UseCases
{
    public class DeleteFile : BaseUseCase<DeleteFileDto, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageAdapter _fileStorageAdapter;
        private readonly IPermissionService _permService;
        private readonly IEventDispatcher _eventDispatcher;

        public DeleteFile(
            IApplicationDbContext dbContext,
            IFileStorageAdapter storageAdapter,
            IPermissionService permService,
            IEventDispatcher eventDispatcher)
        {
            _context = dbContext;
            _eventDispatcher = eventDispatcher;
            _permService = permService;
            _fileStorageAdapter = storageAdapter;
        }

        public async Task<bool> Execute(DeleteFileDto dto)
        {
            if (!dto.Directly)
            {
                var owner = await _permService.GetCurrentUser();

                if (!await _permService.CheckPermission(
                        ZedFileUser
                            .WithId(owner.Id)
                            .CanDeleteFile(ZedFile.WithId(dto.FileId)))
                    ) 
                {
                    return false;
                }
            }
            var file = await _context.Files.FirstOrDefaultAsync(x => x.Id == dto.FileId);

            if (file == null)
            {
                return false;
            }

            await _fileStorageAdapter.DeleteFileAsync(file.FilePath);

            // very important dont remove, it will cause silent deletion ALL
            // of files if someone does hacking stuff in SuperUser user
            await _eventDispatcher.Dispatch(new FileDeleted(file), _context);
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
