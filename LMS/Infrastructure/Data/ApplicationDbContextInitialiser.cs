using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Interfaces;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.Study.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Payment;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data
{
    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

            await initialiser.InitialiseAsync();

            await initialiser.SeedAsync();
        }
    }

    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public ApplicationDbContextInitialiser(
            ILogger<ApplicationDbContextInitialiser> logger,
            ApplicationDbContext context,
            IUserService userService,
            IFileService fileService)
        {
            _fileService = fileService;
            _logger = logger;
            _context = context;
            _userService = userService;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            // Default users
            //var administrator = new UserEntity { UserName = "admin", Email = "admin@localhost" };

            //if (_context.Users.All(u => u.UserName != administrator.UserName))
            //{
            //    await _userService.Create().Execute(new CreateUserDto()
            //    {
            //        UserName = administrator.UserName,
            //        EmailAddress = administrator.Email,
            //        Password = administrator.UserName,
            //        Role = UserRoles.SuperAdmin
            //    });
            //}

            if (!_context.Roles.Any())
            {
                var adminRole = GroupEntity.Create(
                    UserRoles.Admin.ToString(),
                    PermissionEntity.Create(nameof(TicketEntity), "*", PermissionEnum.all),
                    PermissionEntity.Create(nameof(TicketCommentEntity), "*", PermissionEnum.all),
                    PermissionEntity.Create(nameof(PurchaseEntity), "*", PermissionEnum.read),
                    PermissionEntity.Create(nameof(GroupEntity), "*", PermissionEnum.read),
                    PermissionEntity.Create(nameof(TransactionEntity), "*", PermissionEnum.read), 
                    PermissionEntity.Create(nameof(CategoryEntity), "*", PermissionEnum.all), 
                    PermissionEntity.Create(nameof(CourseEntity), "*", PermissionEnum.delete), 
                    PermissionEntity.Create(nameof(NotificationEntity), "*", PermissionEnum.all));

                List<GroupEntity> groups = [
                    adminRole,
                ];
                _context.Groups.AddRange(groups); 
            }

            // Default data
            // Seed, if necessary
            if (!_context.TransactionProviders.Any())
            {
                var providerImages = new Dictionary<string, string>();

                providerImages[nameof(PaymentProviders.BankCardRu)] = "https://playerok.com/images/Icons/CardRF.svg";
                providerImages[nameof(PaymentProviders.Balance)] = "https://playerok.com/images/Icons/Wallet.png";
                providerImages[nameof(PaymentProviders.Test)] = "https://playerok.com/images/Icons/Wallet.png";

                var providers = new List<TransactionProviderEntity>
                {
                    new()
                    {
                        Name = nameof(PaymentProviders.BankCardRu),
                        Systems = [new PaymentSystemEntity() { Name = "paypalych" }]
                    },
                    new() {
                        Name = nameof(PaymentProviders.Balance),
                        Systems = [],
                    },
                    new() {
                        Name = nameof(PaymentProviders.Test),
                        Systems = [],
                    }
                };

                foreach (var provider in providers)
                {
                    provider.Logo = await _fileService.UploadFile().Execute(
                        new Application.Files.Dto.CreateFileDto()
                        {
                            DownloadUrl = providerImages[provider.Name]
                        });
                }

                _context.TransactionProviders.AddRange(providers);

                await _context.SaveChangesAsync();
            }
        }
    }
}
