﻿using Flsurf.Application.Staff.Interfaces;
using Flsurf.Application.Staff.UseCases;

namespace Flsurf.Application.Staff
{
    public class StaffService : IStaffService
    {
        // unused, but could be used in future! 
        private readonly IServiceProvider _serviceProvider;

        public StaffService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public CreateTicket CreateTicket()
        {
            return _serviceProvider.GetRequiredService<CreateTicket>();
        }
        public DeleteTicket DeleteTicket()
        {
            return _serviceProvider.GetRequiredService<DeleteTicket>();
        }
        public UpdateTicket UpdateTicket()
        {
            return _serviceProvider.GetRequiredService<UpdateTicket>();
        }

        public GetTicket GetTicket()
        {
            return _serviceProvider.GetRequiredService<GetTicket>();
        }
        public GetTicketsList GetTicketsList()
        {
            return _serviceProvider.GetRequiredService<GetTicketsList>();
        }

        // news 
        public CreateNews CreateNews()
        {
            return _serviceProvider.GetRequiredService<CreateNews>();
        }
        public UpdateNews UpdateNews()
        {
            return _serviceProvider.GetRequiredService<UpdateNews>();
        }
        public GetNewsById GetNews()
        {
            return _serviceProvider.GetRequiredService<GetNewsById>();
        }
        public GetNewsList GetNewsList()
        {
            return _serviceProvider.GetRequiredService<GetNewsList>();
        }
        public DeleteNews DeleteNews()
        {
            return _serviceProvider.GetRequiredService<DeleteNews>();
        }
    }
}
