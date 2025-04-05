using Flsurf.Application.User.Commands;
using Flsurf.Application.User.Queries;
using Microsoft.AspNetCore.Identity.Data;

namespace Flsurf.Application.User.Interfaces
{
    public interface IUserService
    {
        CreateUserHandler CreateUser();

        UpdateUserHandler UpdateUser();
        GetUserHandler GetUser();
        WarnUserHandler WarnUser();
        GetUsersListHandler GetUsersList();
        SendResetCodeHandler SendResetPasswordCode();
        ResetPasswordHandler ResetPassword();
        GetNotificationsHandler GetNotifications();
        BlockUserHandler BlockUser();
        FindOrCreateExternalUser FindOrCreateExternalUser();
        CreateNotificationHandler CreateNotifications();
    }
}
