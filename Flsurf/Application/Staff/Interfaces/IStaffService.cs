﻿using Flsurf.Application.Staff.UseCases;

namespace Flsurf.Application.Staff.Interfaces
{
    public interface IStaffService
    {
        CreateTicket CreateTicket();
        DeleteTicket DeleteTicket();
        UpdateTicket UpdateTicket();
        GetTicket GetTicket();
        GetTicketsList GetTicketsList();
    }
}
