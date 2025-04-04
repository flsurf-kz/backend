﻿using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetFreelancerProfileHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetFreelancerProfileQuery, FreelancerProfileEntity?>
    {
        private IApplicationDbContext _dbContext = dbContext;

        public async Task<FreelancerProfileEntity?> Handle(GetFreelancerProfileQuery query)
        {
            var profile = await _dbContext.FreelancerProfiles
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.UserId == query.UserId);

            return profile;
        }
    }

}
