using Flsurf.Application.User.Commands;
using Flsurf.Application.User.Interfaces;
using Flsurf.Application.User.Queries;

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

        public CreateUserHandler CreateUser()
        {
            return ServiceProvider.GetRequiredService<CreateUserHandler>();
        }

        public UpdateUserHandler UpdateUser()
        {
            return ServiceProvider.GetRequiredService<UpdateUserHandler>();
        }

        public GetUserHandler GetUser()
        {
            return ServiceProvider.GetRequiredService<GetUserHandler>();
        }
        public GetUsersListHandler GetUsersList()
        {
            return ServiceProvider.GetRequiredService<GetUsersListHandler>();
        }
        public SendResetCodeHandler SendResetPasswordCode()
        {
            return ServiceProvider.GetRequiredService<SendResetCodeHandler>();
        }
        public ResetPasswordHandler ResetPassword()
        {
            return ServiceProvider.GetRequiredService<ResetPasswordHandler>();
        }

        public FindOrCreateExternalUser FindOrCreateExternalUser()
        {
            return ServiceProvider.GetRequiredService<FindOrCreateExternalUser>(); 
        }

        public GetNotificationsHandler GetNotifications()
        {
            return ServiceProvider.GetRequiredService<GetNotificationsHandler>();
        }

        public CreateNotificationHandler CreateNotifications()
        {
            return ServiceProvider.GetRequiredService<CreateNotificationHandler>();
        }

        public WarnUserHandler WarnUser()
        {
            return ServiceProvider.GetRequiredService<WarnUserHandler>();
        }

        public BlockUserHandler BlockUser()
        {
            return ServiceProvider.GetRequiredService<BlockUserHandler>();
        }

        public HideNotifications HideNotifications()
        {
            return ServiceProvider.GetRequiredService<HideNotifications>(); 
        }

        public UpdateTaxSettingsHandler UpdateTaxSettings()
        {
            return ServiceProvider.GetRequiredService<UpdateTaxSettingsHandler>(); 
        }
    }
}
