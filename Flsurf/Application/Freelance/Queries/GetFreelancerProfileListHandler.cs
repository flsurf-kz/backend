using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetFreelancerProfileListHandler(IApplicationDbContext dbContext, IPermissionService permService)
        : IQueryHandler<GetFreelancerProfileListQuery, List<FreelancerProfileEntity>>
    {

        public async Task<List<FreelancerProfileEntity>> Handle(GetFreelancerProfileListQuery query)
        {
            var freelancersQuery = dbContext.FreelancerProfiles.AsQueryable();

            if (query.Skills is not null && query.Skills.Length > 0)
            {
                freelancersQuery = freelancersQuery.Where(
                    f => query.Skills.All(
                        skill => f.Skills.Any(
                            x => x.Id == skill)));  // very slow shit 
            }

            if (query.CostPerHour is not null && query.CostPerHour.Length == 2)
            {
                freelancersQuery = freelancersQuery.Where(f => f.CostPerHour >= query.CostPerHour[0]
                                                             && f.CostPerHour <= query.CostPerHour[1]);
            }

            if (query.ReviewsCount is not null && query.ReviewsCount.Length == 2)
            {
                freelancersQuery = freelancersQuery.Where(f => f.Reviews.Count >= query.ReviewsCount[0]
                                                             && f.Reviews.Count <= query.ReviewsCount[1]);
            }

            if (query.Location is not null)
            {
                freelancersQuery = freelancersQuery.Where(x => x.User.Location != null && x.User.Location == query.Location); 
            }

            if (query.YourHires)
            {
                var currentUser = await permService.GetCurrentUser();
                if (currentUser == null)
                {
                    // Если пользователь не авторизован, "YourHires" вернет пустой список или вызовет ошибку
                    // В зависимости от вашей логики, можно вернуть пустой список или выбросить исключение.
                    // Для простоты, если нет пользователя, то и нанятых им быть не может.
                    return new List<FreelancerProfileEntity>();
                }

                // Получаем ID всех фрилансеров, с которыми у текущего пользователя (как Employer) были контракты
                var contractedFreelancerUserIds = await dbContext.Contracts // Убедитесь, что имя DbSet правильное (Contracts или ContractEntities)
                    .Where(c => c.EmployerId == currentUser.Id) // Текущий пользователь - заказчик
                    .Select(c => c.FreelancerId)                 // Выбираем ID фрилансера из этих контрактов
                    .Distinct()                                  // Только уникальные ID фрилансеров
                    .ToListAsync();

                if (!contractedFreelancerUserIds.Any())
                {
                    // Если заказчик никого не нанимал, возвращаем пустой список
                    return new List<FreelancerProfileEntity>();
                }

                // Фильтруем профили фрилансеров по списку полученных ID
                // FreelancerProfileEntity.UserId связывает профиль с UserEntity.Id фрилансера
                freelancersQuery = freelancersQuery.Where(fp => contractedFreelancerUserIds.Contains(fp.UserId));
            }

            var freelancers = await freelancersQuery
                .OrderByDescending(f => f.Rating) // сортируем по рейтингу
                .Paginate(query.Start, query.Ends)
                .Where(x => x.IsHidden == false)
                .IncludeStandard()
                .ToListAsync();  // very heavy weight 

            return freelancers;
        }
    }

}
