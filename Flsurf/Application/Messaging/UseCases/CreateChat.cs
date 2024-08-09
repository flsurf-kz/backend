using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Application.Messaging.Permissions;
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

            var chat = ChatEntity.Create(name: dto.Name, owner, users, true, ChatTypes.Group);

            await _permService.AddRelationships(
                ZedChat
                    .WithId(chat.Id)
                    .Owner(ZedMessangerUser.WithId(owner.Id)));

            List<Relationship> zedMembers = []; 
            foreach (var user in users)
            {
                zedMembers.Add(
                    ZedChat
                        .WithId(chat.Id)
                        .Member(ZedMessangerUser.WithId(user.Id))); 
            }

            await _permService.AddRelationships(zedMembers);

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return chat.Id;
        }
    }
}
