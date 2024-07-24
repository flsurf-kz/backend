using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Flsurf.Infrastructure.Data;
using Flsurf.Infrastructure.Adapters.FileStorage;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.Adapters.Mailing;
using Flsurf.Infrastructure.Caching;
using Flsurf.Infrastructure.EventStore;
using Flsurf.Infrastructure.Data.Intercepters;
using Flsurf.Application.Study.Interfaces;
using Flsurf.Infrastructure.Adapters.Permissions;
using SpiceDb;

namespace Flsurf.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionString"];
            var filesDirectory = configuration["FilesDirectory"];
            var storageType = configuration["StorageType"];

            Guard.Against.Null(storageType, message: "Storage type is not selected");

            if (Enum.TryParse(storageType, out StorageTypes type))
            {
                if (type == StorageTypes.Local && string.IsNullOrEmpty(filesDirectory))
                {
                    throw new Exception("Files directory is empty, while storage type is local");
                }
            }

            Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.EnableDetailedErrors(true);
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(
                    connectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                );
            });

            services.AddDbContext<EventStoreContext>((sp, options) =>
            {
                options.EnableDetailedErrors(true);
                options.UseNpgsql(
                    connectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                );
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IEventStoreContext>(provider => provider.GetRequiredService<EventStoreContext>());
            services.AddScoped<IEventStore, EventStore.EventStore>();
            services.AddScoped<ApplicationDbContextInitialiser>();
            services.AddScoped<EventStoreContextInitialiser>();

            services.AddSingleton(TimeProvider.System);
            //services.AddScoped<IEventSubscriber<BaseEvent>, LoggingHandler<BaseEvent>>(); 
            services.AddScoped<INotificationCache, InMemoryNotificationCache>();

            services.AddScoped<PasswordService, PasswordService>(
                x =>
                {
                    return new PasswordService(passwordHasher: new PasswordHasher<UserEntity>());
                }
            );

            services.AddMailing(configuration);

            //services.AddScoped<IFileStorageAdapter, S3StorageAdapter>();
            if (type == StorageTypes.Local)
            {
                Guard.Against.Null(filesDirectory, message: "No files directory");

                services.AddScoped<IFileStorageAdapter, LocalFileStorageAdapter>(x => new LocalFileStorageAdapter(filesDirectory));
            }
            else if (type == StorageTypes.Minio)
            {
                var minioConfig = configuration.GetRequiredSection("Minio");

                services.AddScoped<IFileStorageAdapter, MinioStorage>(provider =>
                {
                    var endpoint = minioConfig["Endpoint"];
                    var accessKey = minioConfig["AccessKey"];
                    var secretKey = minioConfig["SecretKey"];
                    var bucketName = minioConfig["BucketName"];

                    if (string.IsNullOrEmpty(endpoint)
                        || string.IsNullOrEmpty(accessKey)
                        || string.IsNullOrEmpty(secretKey)
                        || string.IsNullOrEmpty(bucketName))
                    {
                        throw new ArgumentException("Minio configuration one of the parameters is empty");
                    }

                    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                    // Create a logger
                    var logger = loggerFactory.CreateLogger<MinioStorage>();

                    logger.LogInformation("Creating minio storage");

                    return new MinioStorage(
                        endpoint: endpoint,
                        accessKey: accessKey,
                        secretKey: secretKey,
                        bucketName: bucketName);
                });
            }

            services.AddScoped<ISpiceDbClient, SpiceDbClient>(provider =>
            {
                var serverAddress = configuration["SpiceDbServerAddress"];
                var token = configuration["SpiceDbToken"];
                if (string.IsNullOrEmpty(serverAddress) || string.IsNullOrEmpty(token))
                {
                    throw new ArgumentException("Spice db token or server address is not present");
                }

                var client = new SpiceDbClient(serverAddress, token, null);

                return client; 
            });
            services.AddScoped<IPermissionService, SpiceDbPermService>(); 

            services.AddAuthorizationBuilder();

            return services;
        }
    }
}
