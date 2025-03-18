using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Application.Payment.Exceptions;
using Flsurf.Application.Payment.InnerServices;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Infrastructure.Adapters.Payment;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
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
