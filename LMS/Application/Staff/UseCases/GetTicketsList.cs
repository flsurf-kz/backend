﻿using LMS.Application.Common.Interfaces;
using LMS.Application.Common.UseCases;
using LMS.Application.Staff.Dto;
using LMS.Domain.Staff.Entities;
using LMS.Domain.User.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Staff.UseCases
{
    public class GetTicketsList : BaseUseCase<GetTicketsDto, List<TicketEntity>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAccessPolicy _accessPolicy;

        public GetTicketsList(IApplicationDbContext dbContext, IAccessPolicy accessPolicy)
        {
            _context = dbContext;
            _accessPolicy = accessPolicy;
        }

        public async Task<List<TicketEntity>> Execute(GetTicketsDto dto)
        {
            IQueryable<TicketEntity> query = _context.Tickets;
            var currentUser = await _accessPolicy.GetCurrentUser();

            //if (dto.UserId != currentUser.Id && !await _accessPolicy.CanAccess(UserRoles.Moderator))
            //{
            //    throw new AccessDenied("You are not ticket owner, or something like that");
            //}

            if (dto.UserId != null)
            {
                query = query.Where(x => x.CreatedBy.Id == dto.UserId);
            }

            if (dto.SubjectId != null)
            {
                query = query.Where(x => x.Subject.Id == dto.SubjectId);
            }

            if (dto.IsAssignedToMe == true)
            {
                query = query.Where(x => x.AssignedUser.Id == currentUser.Id);
            }

            var tickets = await query.ToListAsync();

            return tickets;
        }
    }
}
