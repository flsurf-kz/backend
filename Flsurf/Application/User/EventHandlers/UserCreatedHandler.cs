using Flsurf.Application.Common.Events;
using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Domain.User.Events;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.EventHandlers
{
    public class UserCreatedHandler(
        ILogger<UserCreatedHandler> _logger,
        IPermissionService _permService,
        IFreelancerProfileService _freelancerProfileService,
        IClientProfileService _clientProfileService) : IDomainEventSubscriber<UserCreated>
    {
        private const string DEFAULT_PHONE_NUMBER = "7777777777";
        private const string UNKNOWN_USER_TYPE_EXCEPTION = "Неизвестный тип пользователя.";

        public async Task HandleEvent(UserCreated eventValue, IApplicationDbContext _context)
        {
            var user = _context.ChangeTracker
                          .Entries<UserEntity>()
                          .First(e => e.Entity.Id == eventValue.UserId)
                          .Entity;
            if (user == null)
                throw new Exception("CRITICAL: WTF?? Why user does not exists"); 

            var wallet = WalletEntity.Create(user);
            _context.Wallets.Add(wallet);

            _logger.LogInformation($"User created with id: {user.Id}");

            await _permService.AddRelationship(
                ZedWallet.WithId(wallet.Id).Owner(ZedPaymentUser.WithId(user.Id)));

            await CreateProfileByUserType(user);
        }

        private async Task CreateProfileByUserType(UserEntity user)
        {
            switch (user.Type)
            {
                case UserTypes.Freelancer:
                    await CreateFreelancerProfile(user);
                    break;

                case UserTypes.Client:
                    await CreateClientProfile(user);
                    break;

                case UserTypes.Staff:
                case UserTypes.NonUser when user.IsExternalUser:
                    // intentionally no action required
                    break;

                default:
                    throw new DomainException(UNKNOWN_USER_TYPE_EXCEPTION);
            }
        }

        private async Task CreateFreelancerProfile(UserEntity user)
        {
            var command = new Freelance.Commands.FreelancerProfile.CreateFreelancerProfileCommand
            {
                UserId = user.Id,
                Resume = string.Empty,
                Experience = string.Empty,
                HourlyRate = 0
            };

            await _freelancerProfileService.CreateFreelancerProfile().Handle(command);
        }

        private async Task CreateClientProfile(UserEntity user)
        {
            var command = new Freelance.Commands.ClientProfile.CreateClientProfileCommand
            {
                CompanyName = string.Empty,
                EmployerType = ClientType.Indivdual,
                PhoneNumber = user.Phone ?? DEFAULT_PHONE_NUMBER
            };

            await _clientProfileService.CreateClientProfile().Handle(command);
        }
    }

}
