using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Application.Messaging.UseCases
{
    public class InviteMember : BaseUseCase<InviteMemberDto, InvitationEntity>
    {
        public InviteMember() { }

        public async Task<InvitationEntity> Execute(InviteMemberDto dto)
        {
            return new();
        }
    }
}
