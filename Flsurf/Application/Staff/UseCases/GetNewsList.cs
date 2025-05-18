using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Staff.Dto;
using Flsurf.Application.Staff.Perms;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Flsurf.Application.Staff.UseCases
{
    public class GetNewsList : BaseUseCase<GetNewsListDto, ICollection<NewsEntity>>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IPermissionService _perm;

        public GetNewsList(IApplicationDbContext ctx, IPermissionService perm)
        {
            _ctx = ctx;
            _perm = perm;
        }

        public async Task<ICollection<NewsEntity>> Execute(GetNewsListDto dto)
        {
            IQueryable<NewsEntity> q = _ctx.News
                                           .Include(n => n.Attachments)
                                           .OrderByDescending(n => n.PublishTime);

            // диапазон дат
            if (dto.StartDate is not null)
                q = q.Where(n => n.PublishTime >= dto.StartDate);
            if (dto.EndDate is not null)
                q = q.Where(n => n.PublishTime <= dto.EndDate);
            if (dto.ChangeNotes is not null) 
                q = q.Where(x => x.ChangeNotes == dto.ChangeNotes);

            // скрытые новости — только админ
            if (!dto.IncludeHidden)
                q = q.Where(n => !n.IsHidden && n.PublishTime <= DateTime.UtcNow);
            else
            {
                var user = await _perm.GetCurrentUser(); 
                await _perm.EnforceCheckPermission(ZedStaffUser.WithId(user.Id).CanCreateNews());
            }
            q = q.Paginate(dto.Start, dto.Ends);

            return await q.AsNoTracking().ToListAsync();
        }
    }

}
