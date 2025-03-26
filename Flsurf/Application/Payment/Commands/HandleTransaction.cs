using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.InnerServices;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Payment.UseCases
{
    public class HandleTransactionCommand : BaseCommand
    {
        [Required]
        public Guid TransactionId { get; set; }
    }

    // добавляет денег после потверждения из пеймент гейтвея 
    public class HandleTransaction : ICommandHandler<HandleTransactionCommand>
    {
        private IApplicationDbContext _context;
        private TransactionInnerService _innerService; 

        public HandleTransaction(TransactionInnerService innerService, IApplicationDbContext dbContext)
        {
            _context = dbContext;
            _innerService = innerService;
        }

        public async Task<CommandResult> Handle(HandleTransactionCommand dto)
        {
            var result = await _innerService.ConfirmTransaction(dto.TransactionId);
            if (!result.IsSuccess)
                return result; 

            await _context.SaveChangesAsync();
            return result; 
        }
    }
}
