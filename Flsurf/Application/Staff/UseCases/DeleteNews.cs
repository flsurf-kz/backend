using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.Staff.Perms;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Staff.UseCases
{
    public class DeleteNews : BaseUseCase<Guid, bool>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IPermissionService _perm;

        public DeleteNews(IApplicationDbContext ctx, IPermissionService perm)
        {
            _ctx = ctx;
            _perm = perm;
        }

        public async Task<bool> Execute(Guid id)
        {
            var author = await _perm.GetCurrentUser();
            await _perm.EnforceCheckPermission(ZedStaffUser.WithId(author.Id).CanDeleteNews());

            var news = await _ctx.News.FindAsync(id);
            if (news is null) return false;

            _ctx.News.Remove(news);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }

}
