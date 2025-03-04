using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Commands.Category.UpdateCategory;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.User.Events;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Application.Freelance.EventHandlers
{
    public class UserCreatedFreelanceHandler : IEventSubscriber<UserCreated>
    {
        private ILogger _logger;
        private IPermissionService _permService;

        public UserCreatedFreelanceHandler(ILogger<UserCreatedFreelanceHandler> logger, IPermissionService permService)
        {
            _logger = logger;
            _permService = permService;
        }

        public async Task HandleEvent(UserCreated eventValue, IApplicationDbContext _context)
        {
            // TODO? 
            _logger.LogInformation($"Creating freelancer or client account: {eventValue.User.Id}");


            return;
        }
    }
}
