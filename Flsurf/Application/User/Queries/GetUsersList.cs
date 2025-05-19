using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using Minio.Exceptions;

namespace Flsurf.Application.User.Queries
{
    public class GetUsersListQuery : BaseQuery
    {
        /// <summary>
        /// Термин для поиска по Fullname или Email (частичное совпадение, регистронезависимый).
        /// </summary>
        public string? SearchTerm { get; set; } // Раньше был Fullname

        /// <summary>
        /// Фильтр по конкретной роли пользователя.
        /// </summary>
        public UserRoles? Role { get; set; }

        /// <summary>
        /// Строка для продвинутых фильтров (например, "isStaff:true,country:USA").
        /// Доступно только для модераторов и выше.
        /// </summary>
        public string? AdvancedFilters { get; set; }

        /// <summary>
        /// Количество пропускаемых записей (для пагинации).
        /// </summary>
        public int Start { get; set; } = 0;

        /// <summary>
        /// Количество возвращаемых записей (для пагинации).
        /// </summary>
        public int Ends { get; set; } = 10;
    }

    public class GetUsersListHandler : IQueryHandler<GetUsersListQuery, List<UserEntity>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService; // Добавляем сервис для проверки прав

        public GetUsersListHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<List<UserEntity>> Handle(GetUsersListQuery query)
        {
            var currentUser = await _permService.GetCurrentUser(); // Получаем текущего пользователя
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var usersQuery = _context.Users
                                   .IncludeStandard() // Ваше расширение для Include
                                   .AsQueryable();

            // 1. Базовый поиск по SearchTerm (Fullname ИЛИ Email) - доступен всем авторизованным
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                string searchTermLower = query.SearchTerm.ToLower().Trim();
                usersQuery = usersQuery.Where(u =>
                    (u.Fullname != null && u.Fullname.ToLower().Contains(searchTermLower)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchTermLower))
                );
                // Для более высокой производительности на больших объемах данных и сложных текстовых запросах
                // рассмотрите использование Full-Text Search вашей СУБД.
                // Альтернатива с EF.Functions.Like (менее переносимо, зависит от провайдера БД):
                // usersQuery = usersQuery.Where(u =>
                //    (u.Fullname != null && EF.Functions.Like(u.Fullname, $"%{query.SearchTerm}%")) ||
                //    (u.Email != null && EF.Functions.Like(u.Email, $"%{query.SearchTerm}%"))
                // );
                // При использовании Like убедитесь, что ваша БД и collation настроены на регистронезависимый поиск,
                // или используйте ILIKE для PostgreSQL, или LOWER() в запросе.
            }

            // 2. Фильтр по роли - доступен всем авторизованным (если query.Role передан)
            if (query.Role.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.Role == query.Role.Value);
            }

            // 3. Продвинутые фильтры - доступны только модераторам и выше
            if (!string.IsNullOrWhiteSpace(query.AdvancedFilters))
            {
                // Проверка прав на использование продвинутых фильтров
                // Замените UserRoles.Moderator на вашу реальную роль модератора/админа
                // или используйте более гранулярную проверку прав через _permService
                bool canUseAdvancedFilters = currentUser.Role >= UserRoles.Moderator; // Пример проверки по роли

                // if (!await _permService.HasPermissionAsync(currentUser.Id, "use_advanced_user_filters"))
                // {
                //     throw new AccessDeniedException("You do not have permission to use advanced search filters.");
                // }

                if (!canUseAdvancedFilters)
                {
                    throw new AccessDeniedException("You do not have permission to use advanced search filters.");
                }

                usersQuery = ApplyAdvancedFilters(usersQuery, query.AdvancedFilters);
            }

            // 4. Пагинация
            usersQuery = usersQuery.Paginate(query.Start, query.Ends);

            // 5. Сортировка (опционально, можно добавить параметры в DTO)
            // Например, по умолчанию сортировать по Fullname
            usersQuery = usersQuery.OrderBy(u => u.Fullname);

            return await usersQuery.ToListAsync();
        }

        private IQueryable<UserEntity> ApplyAdvancedFilters(IQueryable<UserEntity> query, string advancedFiltersString)
        {
            var filters = advancedFiltersString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var filter in filters)
            {
                var parts = filter.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim().ToLowerInvariant();
                    var value = parts[1].Trim();

                    switch (key)
                    {
                        case "role":
                            if (Enum.TryParse<UserRoles>(value, true, out var roleValue)) // true для регистронезависимого парсинга
                            {
                                query = query.Where(u => u.Role == roleValue);
                            }
                            // else { /* Можно логировать или игнорировать неверное значение роли */ }
                            break;
                        case "isstaff": // Предполагается, что у UserEntity есть поле IsStaff
                            if (bool.TryParse(value, out var isStaffValue))
                            {
                                query = query.Where(u => u.Type == UserTypes.Staff);
                            }
                            // else { /* Логировать/игнорировать */ }
                            break;
                        case "country": // Пример для строкового поля Country в UserEntity
                            if (!string.IsNullOrEmpty(value))
                            {
                                // Регистронезависимый поиск по части строки
                                query = query.Where(u =>
                                    u.Location != null &&
                                    (u.Location.ToString() ?? "").ToLower().Contains(value.ToLower()));
                            }
                            break;
                        // Добавьте другие кейсы для поддерживаемых продвинутых фильтров
                        // Например:
                        // case "registrationdate_after":
                        //     if (DateTime.TryParse(value, out var dateAfter))
                        //     {
                        //         query = query.Where(u => u.CreatedAt >= dateAfter);
                        //     }
                        //     break;
                        default:
                            // Можно логировать неизвестные ключи фильтров или игнорировать
                            break;
                    }
                }
            }
            return query;
        }
    }
}
