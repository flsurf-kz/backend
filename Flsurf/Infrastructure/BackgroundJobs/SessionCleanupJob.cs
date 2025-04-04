using Flsurf.Application.Common.Interfaces;
using Hangfire.Common;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using System;

namespace Flsurf.Infrastructure.BackgroundJobs
{
    public class SessionCleanupJob 
    {
        private readonly IApplicationDbContext _context;

        public SessionCleanupJob(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Execute()
        {
            await _context.SessionTickets
                .Where(s => s.ExpiresAt < DateTime.UtcNow)
                .ExecuteDeleteAsync();
        }
    }
}
