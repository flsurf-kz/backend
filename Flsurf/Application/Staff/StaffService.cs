using Flsurf.Application.Staff.Interfaces;
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

        public CreateComment CreateComment()
        {
            return _serviceProvider.GetRequiredService<CreateComment>();
        }
        public CreateTicket CreateTicket()
        {
            return _serviceProvider.GetRequiredService<CreateTicket>();
        }
        public DeleteComment DeleteComment()
        {
            return _serviceProvider.GetRequiredService<DeleteComment>();
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
        public GetCommentsList GetCommentsList()
        {
            return _serviceProvider.GetRequiredService<GetCommentsList>();
        }
    }
}
