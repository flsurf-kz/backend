using Hangfire;
using Hangfire.PostgreSql;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Presentation.Web.ExceptionHandlers;
using Flsurf.Presentation.Web.Schemas.Filters;
using Flsurf.Presentation.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Flsurf.Infrastructure.Data;
using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http.Features;
using System.Threading.RateLimiting;
using System.Net;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Options;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;

namespace Flsurf.Presentation.Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ILoggingBuilder logging)
        {

            services.AddEndpointsApiExplorer();
            services.AddMvc().AddJsonOptions(c =>
                        c.JsonSerializerOptions.PropertyNamingPolicy
                            = JsonNamingPolicy.CamelCase);
            services.AddSwaggerGen(options =>
            {
                var info = new OpenApiInfo { Title = "SpakOfMind Flsurf", Version = "v1" };
                options.SwaggerDoc(name: "v1", info: info);
                options.AddServer(new() { Url = "http://localhost:8000", Description = "Продакшн" });
                options.AddServer(new() { Url = "http://localhost:8001", Description = "Для тестов" });
                options.EnableAnnotations();
                options.AddSecurityDefinition("session", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Cookie,
                    Name = "session_token",
                    Description = "Session Token"
                });
                options.SchemaFilter<StreamSchemaFilter>();

                options.UseInlineDefinitionsForEnums();

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "session"
                            }
                        },
                        new string[] {}
                    }
                });
            }).AddSwaggerGenNewtonsoftSupport();

            services.AddHangfire(config =>
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(c =>
                        c.UseNpgsqlConnection(configuration["TasksStorage"])));

            services.AddHangfireServer();
            services.AddCoreAdmin();

            services.AddCors(options =>
            {
                options.AddPolicy("FLsurf", builder =>
                {
                    if (environment.IsDevelopment())
                    {
                        builder.SetIsOriginAllowed(_ => true); 
                    } else
                    {
                        builder.WithOrigins("https://flsurf.ru"); 
                    }
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 100 * 1024 * 1024);

            services.AddMemoryCache();

            services.AddHealthChecks();
            services.AddHttpClient();
            if (environment.IsProduction())
            {
                logging.AddSentry(options =>
                {
                    options.Dsn = configuration["SentryDsn"];
                    options.TracesSampleRate = 1.0;
                    options.Environment = "production";
                    options.Release = "app";
                });
            }
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "session_token";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.SessionStore = new DatabaseTicketStore(
                    services.BuildServiceProvider().GetRequiredService<IApplicationDbContext>(),
                    services.BuildServiceProvider().GetRequiredService<IMemoryCache>()
                );
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }
                };
            })
            .AddGoogleOpenIdConnect(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"];
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/signin-google";

                options.Scope.Add("openid"); // Стандартный и обязательный
                options.Scope.Add("profile"); // Этот scope обычно включает: name, family_name, given_name, middle_name, nickname, preferred_username, profile, picture, website, gender, birthdate, zoneinfo, locale, updated_at
                options.Scope.Add("email");   // Для получения email

                options.GetClaimsFromUserInfoEndpoint = true;

                options.CorrelationCookie.SameSite = SameSiteMode.None;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                options.NonceCookie.SameSite = SameSiteMode.None;
                options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
            }); 
            services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
            services.AddRouting();
            services.AddHttpContextAccessor();
            services.AddScoped<IUser, CurrentUser>();

            services.AddHostedService<SwaggerFileUpdater>();

            services.AddAntiforgery(o =>
            {
                o.Cookie.Name = "__Host-flsurf_csrf";
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.SameSite = SameSiteMode.Strict;
                o.Cookie.HttpOnly = true;
                o.HeaderName = "X-CSRF-TOKEN";
            });

            services.AddControllers()
                .AddNewtonsoftJson(opts =>
                {
                    // Внутри фабрики можем получить доступ к IHttpContextAccessor и LinkGenerator
                    var sp = services.BuildServiceProvider();
                    var httpAcc = sp.GetRequiredService<IHttpContextAccessor>();

                    // Регистрируем конвертер только для FileEntity
                    opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; 
                    opts.SerializerSettings.Converters.Add(
                        new FileEntityJsonConverter(httpAcc, environment)
                    );
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                {
                    // лимитируем по IP
                    var ip = context.Connection.RemoteIpAddress ?? IPAddress.Loopback;
                    return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 60,           // 60 запросов
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });
                options.RejectionStatusCode = 429;
            });

            services.AddSignalR();                // <-- SignalR
            services.AddScoped<GeneralHub>();     // DI для HubContext

            return services;
        }
    }
}
