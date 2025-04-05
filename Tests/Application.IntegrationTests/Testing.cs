using Flsurf.Application.Common.cqrs;
using Flsurf.Application.User;
using Flsurf.Application.User.Commands;
using Flsurf.Application.User.Dto;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.IntegrationTests
{
    [SetUpFixture]
    public partial class Testing
    {
        private static ITestDatabase _database;
        private static CustomWebApplicationFactory _factory = null!;
        private static IServiceScopeFactory _scopeFactory = null!;
        private static Guid? _userId;

        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            _database = await TestDatabaseFactory.CreateAsync();

            _factory = new CustomWebApplicationFactory(_database.GetConnection());

            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        public static Guid? GetUserId()
        {
            return _userId;
        }

        public static async Task<Guid> RunAsDefaultUserAsync()
        {
            return await RunAsUserAsync("test@local", "omfg", "Testing1234!", null);
        }

        public static async Task<Guid> RunAsAdministratorAsync()
        {
            return await RunAsUserAsync("administrator@local", "omfg", "Administrator1234!", UserRoles.Admin);
        }

        public static async Task<Guid> RunAsUserAsync(string userName, string surname, string password, UserRoles? role)
        {
            using var scope = _scopeFactory.CreateScope();

            var userService = scope.ServiceProvider.GetRequiredService<UserService>(); 
            CommandResult result = await userService
                .CreateUser()
                .Handle(new CreateUserCommand()
                {
                    Name = userName,
                    Surname = surname, 
                    Password = password, 
                    Role = role, 
                });

            if (!result.IsSuccess) 
                throw new Exception(result.Message);
            if (!Guid.TryParse(result.Id, out var userId))
                throw new Exception("Пиздец");
            _userId = userId;  

            return userId;
        }

        public static async Task ResetState()
        {
            try
            {
                await _database.ResetAsync();
            }
            catch (Exception)
            {
            }

            _userId = null;
        }

        public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
            where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return await context.FindAsync<TEntity>(keyValues);
        }

        public static async Task AddAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Add(entity);

            await context.SaveChangesAsync();
        }

        public static async Task<int> CountAsync<TEntity>() where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return await context.Set<TEntity>().CountAsync();
        }

        [OneTimeTearDown]
        public async Task RunAfterAnyTests()
        {
            await _database.DisposeAsync();
            await _factory.DisposeAsync();
        }
    }
}
