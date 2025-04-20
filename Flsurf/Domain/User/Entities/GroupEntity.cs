using Flsurf.Domain.User.Events;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Flsurf.Domain.User.Entities
{
    public class GroupEntity : BaseAuditableEntity
    {
        [Required]
        public string Name { get; set; } = null!;
        [JsonIgnore]
        public ICollection<UserEntity> Users { get; set; } = [];

        public static GroupEntity Create(string name)
        {
            var group = new GroupEntity()
            {
                Name = name,
                Users = [],
            };

            group.AddDomainEvent(new GroupCreated(group));

            return group;
        }

        public void AddUser(UserEntity user)
        {
            Users.Add(user);

            AddDomainEvent(new GroupUserAdded(this.Id, user.Id));
        }

        public UserEntity? RemoveUser(Guid userId)
        {
            var user = Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return null;
            }

            Users.Remove(user);

            AddDomainEvent(new GroupUserRemoved(this.Id, user.Id)); 
            return user;
        }
    }
}
