using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Interfaces;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.Queries
{
    public class AuthticateWithSecurityAnswerQuery : BaseQuery {  
        public Guid? UserId { get; set; }
        public required string AnswerForQuestion { get; set; } = null!;
        public required SecurityQuestionTypes QuestionType { get; set; }
    }


    public class AuthticateWithSecretAnswerHandler(IApplicationDbContext dbContext, ITokenService tokenService, PasswordService hasherService) 
        : IQueryHandler<AuthticateWithSecurityAnswerQuery, SecurityAnswerDto?>
    {
        public async Task<SecurityAnswerDto?> Handle(AuthticateWithSecurityAnswerQuery query)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == query.UserId);

            if (user == null)
                return null; 

            if (!user.ValidateSecretPhrase(query.AnswerForQuestion, query.QuestionType, hasherService))
                throw new AccessDenied("Invalid secret phrase");

            var token = await tokenService.GenerateTokenAsync(user);
            return new SecurityAnswerDto() { Token = token };
        }
    }
}
