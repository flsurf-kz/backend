using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Messanging.Entities;
using Flsurf.Domain.Messanging.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using SpiceDb.Models;

namespace Flsurf.Application.Messaging.UseCases
{
    public class CreateChat : BaseUseCase<CreateChatDto, Guid>
    {
        private IApplicationDbContext _context { get; set; }
        private IPermissionService _permService { get; set; }

        public CreateChat(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<Guid> Execute(CreateChatDto dto)
        {
            var owner = await _permService.GetCurrentUser(); 

            var users = await _context.Users
                .IncludeStandard()
                .Where(x => dto.UserIds.Contains(x.Id))
                .ToListAsync();
            ContractEntity? contract = null; 
            if (dto.ContractId != null) 
                contract = await _context.Contracts.FirstOrDefaultAsync(x => x.Id == dto.ContractId);

            var chat = ChatEntity.Create(name: dto.Name, owner, users, true, ChatTypes.Group);
            if (contract != null)
            {
                chat.Contracts.Add(contract); 
            }
            await _permService.AddRelationships(
                ZedChat
                    .WithId(chat.Id)
                    .Owner(ZedMessangerUser.WithId(owner.Id)));

            List<Relationship> zedMembers = [];
            if (dto.type == ChatTypes.Group)
            {
                foreach (var user in users)
                {
                    zedMembers.Add(
                        ZedChat
                            .WithId(chat.Id)
                            .Member(ZedMessangerUser.WithId(user.Id)));
                }
            } if (dto.type == ChatTypes.Direct || dto.type == ChatTypes.Support)  // support chat is same as direct
            {
                if (users.Count > 1)
                {
                    throw new Exception("More than two users in private chat"); 
                }

                var perm = ZedChat
                    .WithId(chat.Id)
                    .Member(ZedMessangerUser.WithId(users[0].Id));

                await _permService.AddRelationship(perm);
            }

            await _permService.AddRelationships(zedMembers);

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return chat.Id;
        }
    }
}
