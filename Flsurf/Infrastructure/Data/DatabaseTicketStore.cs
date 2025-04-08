using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System;
using Flsurf.Application.Common.Interfaces;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Flsurf.Domain.User.Entities;

namespace Flsurf.Infrastructure.Data
{
    public class DatabaseTicketStore(IApplicationDbContext _context, IMemoryCache _cache) : ITicketStore
    {
        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid();
            _cache.Set(key, ticket, TimeSpan.FromMinutes(5)); // Кэшируем на 5 минут

            var session = new SessionTicketEntity
            {
                Id = key,
                Value = SerializeTicket(ticket),
                ExpiresAt = ticket.Properties.ExpiresUtc ?? DateTimeOffset.Now.AddHours(1)
            };

            await _context.SessionTickets.AddAsync(session);
            await _context.SaveChangesAsync();

            return key.ToString();
        }

        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            if (_cache.TryGetValue(key, out AuthenticationTicket? cachedTicket))
                return cachedTicket;

            if (!Guid.TryParse(key, out var keyId))
                return null;

            var session = await _context.SessionTickets
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == keyId);

            if (session == null) return null;

            var ticket = DeserializeTicket(session.Value);
            if (ticket == null) return null;
            _cache.Set(key, ticket, TimeSpan.FromMinutes(5));

            return ticket;
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            if (!Guid.TryParse(key, out var keyGuid))
                throw new ArgumentException("Invalid key format", nameof(key));

            var session = await _context.SessionTickets.FindAsync(keyGuid);
            if (session != null)
            {
                session.Value = SerializeTicket(ticket);
                // Приводим время к UTC
                session.ExpiresAt = ticket.Properties.ExpiresUtc.HasValue
                    ? ticket.Properties.ExpiresUtc.Value.ToUniversalTime()
                    : DateTimeOffset.UtcNow.AddMinutes(60);

                _context.SessionTickets.Update(session);
                await _context.SaveChangesAsync();
            }
        }


        public async Task RemoveAsync(string key)
        {
            if (!Guid.TryParse(key, out var keyGuid))
                throw new ArgumentException("Invalid key format", nameof(key));

            var session = await _context.SessionTickets.FindAsync(keyGuid);
            if (session != null)
            {
                _context.SessionTickets.Remove(session);
                await _context.SaveChangesAsync();
            }
        }

        private static byte[] SerializeTicket(AuthenticationTicket ticket)
        {
            return TicketSerializer.Default.Serialize(ticket);
        }

        private static AuthenticationTicket? DeserializeTicket(byte[] data)
        {
            return TicketSerializer.Default.Deserialize(data);
        }
    }
}
