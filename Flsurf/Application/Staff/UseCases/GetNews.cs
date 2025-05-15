using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Staff.UseCases
{
    public class GetNewsById : BaseUseCase<Guid, NewsEntity?>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IPermissionService _perm;

        public GetNewsById(IApplicationDbContext ctx, IPermissionService perm)
        {
            _ctx = ctx;
            _perm = perm;
        }

        public async Task<NewsEntity?> Execute(Guid id)
        {
            var news = await _ctx.News
                                 .Include(n => n.Attachments)
                                 .FirstOrDefaultAsync(n => n.Id == id);

            if (news == null) return null;

            if (news.IsHidden || news.PublishTime > DateTime.UtcNow)
                await _perm.EnforceCheckPermission(ZedAdmin.WithCurrent());

            return news;
        }
    }

}
