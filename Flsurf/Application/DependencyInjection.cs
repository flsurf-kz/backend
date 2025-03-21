﻿using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Files;
using Flsurf.Application.Payment;
using Flsurf.Application.Staff;
using Flsurf.Application.User;
using Flsurf.Infrastructure;
using Flsurf.Infrastructure.EventDispatcher;
using System.Reflection;

namespace Flsurf.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, ConfigurationManager config)
        {
            services.AddSingleton<IEventDispatcher, EventDispatcher>();
            services.AddUserApplicationServices();
            services.AddPaymentApplicationServices(config);
            services.AddFilesApplicationServices();
            services.AddStaffApplicationServices();

            services.AddUseCasesFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
