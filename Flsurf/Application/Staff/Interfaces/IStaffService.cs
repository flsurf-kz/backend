﻿using Flsurf.Application.Staff.UseCases;

namespace Flsurf.Application.Staff.Interfaces
{
    public interface IStaffService
    {
        CreateComment CreateComment();
        CreateTicket CreateTicket();
        DeleteComment DeleteComment();
        DeleteTicket DeleteTicket();
        UpdateTicket UpdateTicket();
        GetTicket GetTicket();
        GetTicketsList GetTicketsList();
        GetCommentsList GetCommentsList();
    }
}
