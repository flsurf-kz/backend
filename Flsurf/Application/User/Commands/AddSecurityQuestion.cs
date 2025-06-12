using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.Commands
{
    // TODO: use token 
    public class AddSecurityQuestionCommand : BaseCommand { 
        public required Guid UserId { get; set; }
        public required string SecurityAnswer { get; set; } = null!; 
        public required SecurityQuestionTypes Type { get; set; }
    }


    public class AddSecurityQuestionHandler(IApplicationDbContext dbContext, PasswordService hasherService) : ICommandHandler<AddSecurityQuestionCommand>
    {
        public async Task<CommandResult> Handle(AddSecurityQuestionCommand cmd)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == cmd.UserId);
            if (user == null)
                return CommandResult.NotFound("Not found", cmd.UserId);

            user.AddSecretPhrase(cmd.SecurityAnswer, cmd.Type, hasherService);

            return CommandResult.Success(user.Id); 
        }
    }
}
