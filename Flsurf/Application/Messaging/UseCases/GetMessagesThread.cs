using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetMessagesThreadDto
    { 
        public Guid MessageId { get; set; }
    }

    // отправляет цепочку ответов для любого сообщения (лимит 10 сообщении) 
    public class MessageThreadDto
    {  
        public Guid MessageId { get; set; }
        public Guid ReplyToId { get; set; }
    }

    // Не нужна верификация прав
    public class GetMessagesThread : BaseUseCase<GetMessagesThreadDto, List<MessageThreadDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private const int _limit = 10;

        public GetMessagesThread(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MessageThreadDto>> Execute(GetMessagesThreadDto dto)
        {
            // Результирующий список пар (текущее сообщение → сообщение, на которое оно отвечает)
            var result = new List<MessageThreadDto>(_limit);
            // Начинаем с переданного MessageId
            Guid currentId = dto.MessageId;

            for (int i = 0; i < _limit; i++)
            {
                // Берём только нужные поля: Id и ссылку на родительское сообщение
                var msg = await _dbContext.Messages
                    .Where(m => m.Id == currentId)
                    .Select(m => new { m.Id, m.ReplyedToMessageId })
                    .FirstOrDefaultAsync();

                // Если такого сообщения нет или у него нет родителя — выходим
                if (msg == null || !msg.ReplyedToMessageId.HasValue)
                    break;

                // Добавляем пару (текущий → его родитель)
                result.Add(new MessageThreadDto() { MessageId = msg.Id, ReplyToId = msg.ReplyedToMessageId.Value });

                // Переходим вверх по цепочке
                currentId = msg.ReplyedToMessageId.Value;
            }

            return result;
        }
    }
}
