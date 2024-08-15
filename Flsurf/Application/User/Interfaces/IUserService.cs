using Flsurf.Application.User.UseCases;
using Microsoft.AspNetCore.Identity.Data;

namespace Flsurf.Application.User.Interfaces
{
    public interface IUserService
    {
        public abstract CreateUser Create();

        public abstract UpdateUser Update();
        public abstract GetUser Get();
        public abstract WarnUser WarnUser();
        public abstract GetUsersList GetList();
        public abstract SendResetCode SendResetPasswordCode();
        public abstract ResetPassword ResetPassword();
        public abstract GetNotifications GetNotifications();
        public abstract CreateNotification CreateNotification();
    }
}
