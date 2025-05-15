using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Staff.Dto;
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
            await _perm.EnforceCheckPermission(ZedAdmin.WithCurrent());

            var author = await _perm.GetCurrentUser();
            var atts = await _files.UploadFiles().Execute(dto.Files);

            var news = NewsEntity.Create(dto.Title,
                                         dto.Text,
                                         (List<FileEntity>)atts,
                                         dto.PublishTime,
                                         author);

            await _ctx.News.AddAsync(news);
            await _ctx.SaveChangesAsync();
            return news;
        }
    }

}
