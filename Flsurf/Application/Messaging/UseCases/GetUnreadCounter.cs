using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetUnreadCounterDto // DTO for the use case input
    {
        public Guid? ChatId { get; set; }
        // UserId can be passed or fetched from IPermissionService if always for current user
        public Guid? UserId { get; set; }
    }

    public sealed class GetUnreadCounter(
            IApplicationDbContext db,
            IPermissionService perm)
        : BaseUseCase<GetUnreadCounterDto, int>
    {
        public async Task<int> Execute(GetUnreadCounterDto dto)
        {
            // ── текущий пользователь ────────────────────────────────────
            Guid userId = dto.UserId ?? (await perm.GetCurrentUser()).Id;

            // ── только один чат ─────────────────────────────────────────
            if (dto.ChatId is Guid chatId)
                return await CountForSingleChat(chatId, userId);

            // ── ВСЕ чаты пользователя -----------------------------------

            // 1. Id всех чатов, где он владелец либо в Participants
            var myChatsIds = await db.Chats
                .Where(c => c.OwnerId == userId ||
                            c.Participants.Any(p => p.Id == userId))
                .Select(c => c.Id)
                .ToListAsync();

            if (myChatsIds.Count == 0)
                return 0;

            // 2. Последние точки чтения по каждому чату
            var lastReads = await db.MessageReads
                .Where(mr => mr.ReadById == userId && myChatsIds.Contains(mr.ChatId))
                .GroupBy(mr => mr.ChatId)
                .Select(g => new { g.Key, LastRead = g.Max(x => x.ReadAt) })
                .ToListAsync();

            /* -------- подсчёт -------- */

            // 2а. Чаты, в которых уже есть отметка «прочитал»
            var lastReadsDict = lastReads.ToDictionary(lr => lr.Key, lr => lr.LastRead);

            var unreadAfterRead = await db.Messages
                .Where(m => myChatsIds.Contains(m.ChatId) &&
                            m.SenderId != userId &&                 // не свои
                            lastReadsDict.Keys.Contains(m.ChatId) &&// только те чаты
                            m.SentDate > lastReadsDict[m.ChatId])
                .CountAsync();

            // 2б. Чаты, где отметки чтения нет → всё непрочитанное
            var neverReadChatIds = myChatsIds.Except(lastReadsDict.Keys).ToList();

            var unreadNeverRead = await db.Messages
                .Where(m => neverReadChatIds.Contains(m.ChatId) &&
                            m.SenderId != userId)
                .CountAsync();

            return unreadAfterRead + unreadNeverRead;
        }

        /* ---------- helper (один чат) --------------------------------*/
        private async Task<int> CountForSingleChat(Guid chatId, Guid userId)
        {
            // проверяем, что пользователь действительно состоит в чате
            bool member = await db.Chats.AnyAsync(c =>
                c.Id == chatId &&
                (c.OwnerId == userId || c.Participants.Any(p => p.Id == userId)));
            if (!member) return 0;

            var lastRead = await db.MessageReads
                .Where(mr => mr.ChatId == chatId && mr.ReadById == userId)
                .OrderByDescending(mr => mr.ReadAt)
                .Select(mr => (DateTime?)mr.ReadAt)
                .FirstOrDefaultAsync();

            var query = db.Messages.Where(m => m.ChatId == chatId &&
                                               m.SenderId != userId);
            if (lastRead.HasValue)
                query = query.Where(m => m.SentDate > lastRead.Value);

            return await query.CountAsync();
        }
    }
}
