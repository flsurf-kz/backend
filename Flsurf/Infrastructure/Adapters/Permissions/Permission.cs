namespace Flsurf.Infrastructure.Adapters.Permissions
{
    public class Permission
    {
        public readonly string objectId;
        public readonly string objectName;
        public readonly string relation;
        public readonly string userId;
        public readonly string userName;
        public readonly string objectPrefix;
        public readonly string userPrefix;

        public Permission(string fullperm) {
            // example: arch/document:firstdoc#reader@arch/user:bob
            relation = fullperm.Split('#', '@')[1];
            var object_string = fullperm.Split('#')[0].Split(":");
            objectName = object_string[0]; 
            objectId = object_string[1];

            var user_string = fullperm.Split("@")[1].Split(":"); 
            userName = user_string[0];
            userId = user_string[1];
        }

        public Permission(string objectId, string objectName, string relation, string userId, string userName) 
        {
            this.objectName = objectName;
            this.relation = relation;
            this.userId = userId;
            this.userName = userName;
            this.objectId = objectId;
        }
    }
}
