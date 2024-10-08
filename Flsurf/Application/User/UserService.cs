﻿using Flsurf.Application.User.Interfaces;
using Flsurf.Application.User.UseCases;

namespace Flsurf.Application.User
{
    public class UserService : IUserService
    {
        private readonly IServiceProvider ServiceProvider;

        public UserService(
            IServiceProvider serviceProvider
        )
        {
            ServiceProvider = serviceProvider;
        }

        public CreateUser Create()
        {
            return ServiceProvider.GetRequiredService<CreateUser>();
        }

        public UpdateUser Update()
        {
            return ServiceProvider.GetRequiredService<UpdateUser>();
        }

        public GetUser Get()
        {
            return ServiceProvider.GetRequiredService<GetUser>();
        }
        public GetUsersList GetList()
        {
            return ServiceProvider.GetRequiredService<GetUsersList>();
        }
        public SendResetCode SendResetPasswordCode()
        {
            return ServiceProvider.GetRequiredService<SendResetCode>();
        }
        public ResetPassword ResetPassword()
        {
            return ServiceProvider.GetRequiredService<ResetPassword>();
        }

        public GetNotifications GetNotifications()
        {
            return ServiceProvider.GetRequiredService<GetNotifications>();
        }

        public CreateNotification CreateNotification()
        {
            return ServiceProvider.GetRequiredService<CreateNotification>();
        }

        public WarnUser WarnUser()
        {
            return ServiceProvider.GetRequiredService<WarnUser>();
        }
    }
}
