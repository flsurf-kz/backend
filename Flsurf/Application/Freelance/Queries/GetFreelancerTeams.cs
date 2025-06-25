using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries;

/// <summary>Возвращает команды, участником или владельцем которых является текущий пользователь.</summary>
public sealed class GetFreelancerTeamsListQuery : BaseQuery
{
    /// <param name="start">Смещение в списке (по умолчанию 0).</param>
    /// <param name="take">Сколько записей вернуть (по умолчанию 10).</param>
    public GetFreelancerTeamsListQuery(int start = 0, int take = 10)
    {
        Start = start;
        Take  = take;
    }

    public int Start { get; }
    public int Take  { get; }
}

public sealed class GetFreelancerTeamsHandler(
    IApplicationDbContext db,
    IPermissionService   perm)
    : IQueryHandler<GetFreelancerTeamsListQuery, List<FreelancerTeamEntity>>
{
    public async Task<List<FreelancerTeamEntity>> Handle(GetFreelancerTeamsListQuery q)
    {
        var user  = await perm.GetCurrentUser();
        var uid   = user.Id;

        /*  Берём те команды, где пользователь = владелец
            ИЛИ числится в списке participants                                     */
        var teams = await db.FreelancerTeams
            .IncludeStandard()                         // Avatar, participants, owner и т.п.
            .Where(t => t.OwnerId == uid ||
                        t.Participants.Any(p => p.Id == uid))
            .OrderBy(t => t.Name)                      // произвольная сортировка
            .Skip(q.Start)
            .Take(q.Take)
            .ToListAsync();

        return teams;
    }
}