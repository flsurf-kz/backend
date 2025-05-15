using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class UpdateNews : BaseUseCase<UpdateNewsDto, NewsEntity>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IFileService _files;
        private readonly IPermissionService _perm;

        public UpdateNews(IApplicationDbContext ctx,
                          IFileService files,
                          IPermissionService perm)
        {
            _ctx = ctx;
            _files = files;
            _perm = perm;
        }

        public async Task<NewsEntity> Execute(UpdateNewsDto dto)
        {
            var user = await _perm.GetCurrentUser(); 

            await _perm.EnforceCheckPermission(ZedStaffUser.WithId(user.Id).CanUpdateNews());

            var news = await _ctx.News
                                 .Include(n => n.Attachments)
                                 .FirstAsync(n => n.Id == dto.NewsId);

            if (dto.Title != null) news.Title = dto.Title;
            if (dto.Text != null) news.Text = dto.Text;
            if (dto.PublishTime != null) news.PublishTime = dto.PublishTime.Value;
            if (dto.IsHidden != null) news.IsHidden = dto.IsHidden.Value;

            if (dto.NewFiles?.Any() == true)
            {
                var uploaded = await _files.UploadFiles().Execute(dto.NewFiles);
                news.Attachments ??= new List<FileEntity>();
                foreach (var f in uploaded) news.Attachments.Add(f);
            }

            await _ctx.SaveChangesAsync();
            return news;
        }
    }

}
