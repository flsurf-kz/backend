using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.Events;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.FileStorage;
using Flsurf.Infrastructure.EventDispatcher;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Files.UseCases
{
    public class DeleteFile : BaseUseCase<DeleteFileDto, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageAdapter _fileStorageAdapter;
        private readonly IAccessPolicy _accessPolicy;
        private readonly IEventDispatcher _eventDispatcher;

        public DeleteFile(
            IApplicationDbContext dbContext,
            IFileStorageAdapter storageAdapter,
            IAccessPolicy accessPolicy,
            IEventDispatcher eventDispatcher)
        {
            _context = dbContext;
            _eventDispatcher = eventDispatcher;
            _accessPolicy = accessPolicy;
            _fileStorageAdapter = storageAdapter;
        }

        public async Task<bool> Execute(DeleteFileDto dto)
        {
            if (!dto.Directly)
            {
                if (!await _accessPolicy.Role(UserRoles.Admin))
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
