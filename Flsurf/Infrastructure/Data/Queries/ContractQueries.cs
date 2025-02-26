using Flsurf.Domain.Freelance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class ContractQueries
    {
        public static IQueryable<ContractEntity> IncludeStandard(this IQueryable<ContractEntity> query)
        {
            return query
                .Include(c => c.Job)       // Включаем работу
                .Include(c => c.Employer)    // Включаем клиента
                .Include(c => c.Freelancer) // Включаем фрилансера
                .Include(c => c.Job.Employer) // Загружаем заказчика работы
                .Include(c => c.Job.RequiredSkills); // Загружаем требуемые навыки
        }
    }

}
