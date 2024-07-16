using Microsoft.OpenApi.Any;
using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.User.Events
{
    public class UserUpdated(
        UserEntity user,
        string fieldName
    ) : BaseEvent
    {
        public string FieldName { get; set; } = fieldName;
        public UserEntity User { get; } = user;
    }
}
