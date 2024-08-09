using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;

namespace Flsurf.Application.Messaging.UseCases
{
    public class KickMember : BaseUseCase<KickMemberDto, bool>
    {
        public KickMember()
        {
        }

        public async Task<bool> Execute(KickMemberDto dto)
        {
            return true;
        }
    }
}
