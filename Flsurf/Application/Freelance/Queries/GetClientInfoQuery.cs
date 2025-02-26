﻿using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetClientInfoQuery : BaseQuery
    {
        public Guid UserId { get; set; }
    }
}
