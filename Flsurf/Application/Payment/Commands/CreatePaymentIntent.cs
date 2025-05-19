using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Infrastructure.Adapters.Payment;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.Payment.Commands
{
    // Команда для запроса client_secret для Stripe SetupIntent
    public class CreateSetupIntentCommand // Возвращает CardSetupDetails
    {
        // ID провайдера (например, Stripe) из вашей базы данных TransactionProviderEntity.Id
        public Guid ProviderId { get; set; }

        // ID конкретной платежной системы этого провайдера (например, "Cards" для Stripe)
        // Это может быть PaymentSystemEntity.Id, если ваш адаптер или логика это использует.
        // Если для Stripe это не нужно, или провайдер имеет только одну систему, это поле может быть опциональным
        // или определяться на бэкенде.
        public Guid? SystemId { get; set; } // Опционально, зависит от вашей реализации IPaymentAdapterFactory/IPaymentAdapter

        // URL, на который Stripe может вернуть пользователя (например, после 3D Secure)
        // Это должно быть передано с фронтенда, так как URL может быть динамическим
        // или содержать параметры сессии.
        public string ReturnUrl { get; set; } = string.Empty;

        // Дополнительные метаданные, если Stripe их поддерживает для SetupIntents
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class CreateSetupIntentHandler : BaseUseCase<CreateSetupIntentCommand, CardSetupDetails>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IPaymentAdapterFactory _paymentAdapterFactory;

        public CreateSetupIntentHandler(
            IApplicationDbContext dbContext,
            IPermissionService permissionService,
            IPaymentAdapterFactory paymentAdapterFactory)
        {
            _dbContext = dbContext;
            _permissionService = permissionService;
            _paymentAdapterFactory = paymentAdapterFactory;
        }

        public async Task<CardSetupDetails> Handle(CreateSetupIntentCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _permissionService.GetCurrentUser(cancellationToken)
                ?? throw new UnauthorizedAccessException("Пользователь не авторизован.");

            var provider = await _dbContext.TransactionProviders
                .FirstOrDefaultAsync(p => p.Id == request.ProviderId, cancellationToken)
                ?? throw new NotFoundException($"Платежный провайдер с ID {request.ProviderId} не найден.");

            // Здесь логика выбора конкретной "системы" (PaymentSystemEntity) для провайдера, если это необходимо.
            // Если systemId не используется или Stripe адаптер сам знает, какую систему использовать (например, "card"),
            // то этот шаг может быть проще.
            // Для Stripe, обычно название адаптера/провайдера уже определяет, что это Stripe.
            var paymentAdapter = _paymentAdapterFactory.GetPaymentAdapter(provider.Name); // provider.Name должен быть "Stripe" или аналогично

            // Получаем Stripe Customer ID для текущего пользователя, если он уже существует.
            // Это важно для Stripe, чтобы привязывать способы оплаты к одному клиенту.
            // Вам нужно будет хранить Stripe Customer ID в вашей UserEntity или связанной сущности.
            // Предположим, что UserEntity имеет поле StripeCustomerId.
            string? stripeCustomerId = currentUser.StripeCustomerId; // ЗАМЕНИТЕ НА РЕАЛЬНОЕ ПОЛЕ

            var cardSetupRequest = new PrepareCardSetupRequest
            {
                UserId = currentUser.Id,
                CustomerIdInProvider = stripeCustomerId, // Передаем Stripe Customer ID, если есть
                ReturnUrl = request.ReturnUrl,
                Metadata = request.Metadata
            };

            var cardSetupDetails = await paymentAdapter.PrepareCardSetupAsync(cardSetupRequest);

            if (!cardSetupDetails.Success)
            {
                // Можно логировать ошибку или выбросить более специфическое исключение
                throw new ApplicationException(cardSetupDetails.ErrorMessage ?? "Не удалось подготовить настройку карты на стороне платежного провайдера.");
            }

            // Если Stripe Customer ID не было и адаптер его создал (некоторые адаптеры могут это делать),
            // его нужно сохранить для пользователя. Stripe адаптер должен был бы вернуть новый Customer ID
            // в CardSetupDetails или через другой механизм, чтобы вы могли его сохранить.
            // Например, если CardSetupDetails содержит newCustomerIdInProvider:
            // if (string.IsNullOrEmpty(stripeCustomerId) && !string.IsNullOrEmpty(cardSetupDetails.NewCustomerIdInProvider))
            // {
            //     currentUser.StripeCustomerId = cardSetupDetails.NewCustomerIdInProvider;
            //     _dbContext.Users.Update(currentUser); // Или ваш способ обновления пользователя
            //     await _dbContext.SaveChangesAsync(cancellationToken);
            // }


            return cardSetupDetails;
        }
    }
}
