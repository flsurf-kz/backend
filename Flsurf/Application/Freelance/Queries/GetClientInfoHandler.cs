using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Queries.Responses;
using Flsurf.Domain.Freelance.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetClientOrderInfoHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetClientInfoQuery, ClientOrderInfo?>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<ClientOrderInfo?> Handle(GetClientInfoQuery query)
        {
            var client = await _dbContext.ClientProfiles
                .Include(c => c.User) // Загружаем пользователя
                    .ThenInclude(x => x.Avatar)
                .Include(c => c.Jobs) // Загружаем все опубликованные заказы
                .Include(c => c.Contracts) // Загружаем все контракты клиента
                .Where(c => c.UserId == query.UserId)
                .Select(c => new ClientOrderInfo
                {
                    UserId = c.UserId,
                    Name = c.User.Fullname,
                    Avatar = c.User.Avatar,
                    IsVerified = c.IsPhoneVerified,

                    // Статистика заказчика
                    ActiveJobs = c.Jobs.Count(j => j.Status == JobStatus.Open),
                    ClosedJobs = c.Jobs.Count(j => j.Status == JobStatus.Closed),
                    ArbitrationJobs = c.Jobs.Count(j => j.Status == JobStatus.Expired),

                    ActiveContracts = c.Contracts.Count(con => con.Status == ContractStatus.Active),
                    CompletedContracts = c.Contracts.Count(con => con.Status == ContractStatus.Completed),
                    ArbitrationContracts = c.Contracts.Count(con => con.Status == ContractStatus.Paused),

                    // Даты
                    RegisteredAt = c.CreatedAt.ToString() ?? "Неизвестно",
                    LastActiveAt = c.LastActiveAt.ToString() ?? "Неизвестно",

                    // Верификация
                    IsPhoneVerified = c.IsPhoneVerified,
                })
                .FirstOrDefaultAsync();

            return client;
        }
    }

}
