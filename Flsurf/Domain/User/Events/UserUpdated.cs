using Microsoft.OpenApi.Any;
using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.User.Events
{
    public class UserUpdated(UserEntity user, string fieldName) : BaseEvent
    {
        public Guid UserId { get; } = user.Id;
        public string FieldName { get; } = fieldName;
    }

}
