﻿using Flsurf.Application.User.Permissions;
using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedJob : ResourceReference
    {
        private ZedJob(string jobId) : base($"flsurf/job:{jobId}") { }

        public static ZedJob WithId(Guid id) => new(id.ToString());

        public static ZedJob WithWildcard() => new("*"); 

        public Relationship Owner(ZedUser user) => new(user, "owner", this);
    }
}
