using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Staff.UseCases
{
    public class CreateNews : BaseUseCase<CreateNewsDto, NewsEntity>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IFileService _files;
        private readonly IPermissionService _perm;

        public CreateNews(IApplicationDbContext ctx,
                          IFileService files,
                          IPermissionService perm)
        {
            _ctx = ctx;
            _files = files;
            _perm = perm;
        }

        public async Task<NewsEntity> Execute(CreateNewsDto dto)
        {
            var author = await _perm.GetCurrentUser();
            await _perm.EnforceCheckPermission(ZedStaffUser.WithId(author.Id).CanCreateNews());


            var news = NewsEntity.Create(dto.Title,
                                         dto.Text, 
                                         [], 
                                         dto.PublishTime,
                                         author, 
                                         dto.ChangeNotes);


            if (dto.Files != null)
                news.Attachments = await _files.UploadFiles().Execute(dto.Files);

            await _ctx.News.AddAsync(news);
            await _ctx.SaveChangesAsync();
            return news;
        }
    }

}
