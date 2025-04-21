using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.User.Enums;
using Flsurf.Domain.User.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Commands
{
    public class UpdateTaxSettingsCommand : BaseCommand {
        [Required] 
        public Guid UserId { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string CountryIso { get; set; } = null!;

        [Required]
        [StringLength(12, MinimumLength = 9)]
        public string LocalIdNumber { get; set; } = null!; 

        [Required] 
        public LegalStatus LegalStatus { get; set; }
        [Required] 
        public TaxRegime TaxRegime { get; set; }

        public bool VatRegistered { get; set; }
        public string VatNumber { get; set; } = null!;

        // Банковские реквизиты
        [Required] public string BankBic { get; set; } = null!;
        [Required] public string BankAccountNumber { get; set; } = null!;
        [Required] public string BankName { get; set; } = null!; 
    }

    public class UpdateTaxSettingsHandler(IApplicationDbContext dbContext, IPermissionService permService
        ) : ICommandHandler<UpdateTaxSettingsCommand>
    {
        public async Task<CommandResult> Handle(UpdateTaxSettingsCommand cmd)
        {
            var current = await permService.GetCurrentUser();

            var user = await dbContext.Users
                .Include(u => u.TaxInfo)
                .FirstOrDefaultAsync(u => u.Id == cmd.UserId);

            if (user == null)
                return CommandResult.NotFound("Пользователь не найден", cmd.UserId);

            // Собираем новый VO
            var bank = new BankDetails(cmd.BankBic, cmd.BankAccountNumber, cmd.BankName);
            var tp = new TaxInformation(
                cmd.CountryIso,
                cmd.LocalIdNumber,
                cmd.LegalStatus,
                cmd.TaxRegime,
                cmd.VatRegistered,
                cmd.VatNumber,
                bank
            );

            // Присваиваем
            user.TaxInfo = tp;

            await dbContext.SaveChangesAsync();
            return CommandResult.Success(user.Id);
        } 
    }
}
