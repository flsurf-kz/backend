﻿using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using SpiceDb.Models;

namespace Flsurf.Application.Messaging.UseCases
{
    public class GetChatsList : BaseUseCase<GetChatsListDto, ICollection<ChatEntity>>
    {
        private IApplicationDbContext _context { get; set; }
        private IPermissionService _permService { get; set; }

        public GetChatsList(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<ICollection<ChatEntity>> Execute(GetChatsListDto dto)
        {
            var user = await _permService.GetCurrentUser();
            if (dto.UserId != null && dto.UserId != user.Id)
                throw new AccessDenied("User id is not equal to chat ids");

            // include moderators chats! 
            var perms = _permService.LookupSubjects(
                new ResourceReference("user", user.Id.ToString()), 
                "read", 
                "chat");

            List<Guid> chatIds = [];
            Guid chatId; 

            await foreach (var perm in perms)
            {
                if (Guid.TryParse(perm.Subject.Id, out chatId)) {
                    chatIds.Add(chatId); 
                }
            }

            var chats = await _context.Chats
                .IncludeStandard()
                .Where(x => chatIds.Contains(x.Id))
                .ToListAsync(); 

            return chats;
        }
    }
}
