using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Infrastructure.Adapters.Payment;
using Flsurf.Infrastructure.Adapters.Payment.Systems;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Flsurf.Application.Payment.Commands
{
    // Команда для запроса client_secret для Stripe SetupIntent
    public class CreateSetupIntentCommand // Возвращает CardSetupDetails
    {
        // ID провайдера (например, Stripe) из вашей базы данных TransactionProviderEntity.Id
        [Required]
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
        private readonly IPermissionService _permissionService; // ЗАМЕНА: ICurrentUserIdentifier на IPermissionService
        private readonly IPaymentAdapterFactory _paymentAdapterFactory;
        private readonly HttpClient _httpClientForStripe;

        public CreateSetupIntentHandler(
            IApplicationDbContext dbContext,
            IPermissionService permissionService, // ЗАМЕНА
            IPaymentAdapterFactory paymentAdapterFactory,
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _dbContext = dbContext;
            _permissionService = permissionService; // ЗАМЕНА
            _paymentAdapterFactory = paymentAdapterFactory;

            _httpClientForStripe = httpClientFactory.CreateClient("StripeClient"); // Имя должно совпадать с регистрацией в DI

            var secretKey = config["Payments:Stripe:SecretKey"]; 
            // Установка заголовка авторизации для HttpClient один раз в конструкторе
            if (secretKey is not null)
            {
                _httpClientForStripe.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", secretKey);
            }
            else
            {
                // Логирование или выброс исключения, если SecretKey не задан
                throw new InvalidOperationException("Stripe SecretKey не сконфигурирован.");
            }
        }

        public async Task<CardSetupDetails> Execute(CreateSetupIntentCommand request)
        {
            var user = await _permissionService.GetCurrentUser(); 

            var provider = await _dbContext.TransactionProviders
                .FirstOrDefaultAsync(p => p.Id == request.ProviderId)
                ?? throw new DomainException($"Платежный провайдер с ID {request.ProviderId} не найден.");

            var paymentAdapter = _paymentAdapterFactory.GetPaymentAdapter(provider.Name);

            var stripeMapping = await _dbContext.UserPaymentGatewayCustomers
                .FirstOrDefaultAsync(m => m.UserId == user.Id && m.PaymentProviderId == provider.Id);

            string? stripeCustomerId = stripeMapping?.CustomerIdInProvider;

            if (string.IsNullOrEmpty(stripeCustomerId))
            {
                var customerData = new Dictionary<string, string>
                {
                    ["email"] = user.Email, // UserEntity должен иметь Email
                    ["name"] = user.Fullname, // UserEntity должен иметь Fullname
                    [$"metadata[flsurf_user_id]"] = user.Id.ToString()
                };

                var customerResp = await _httpClientForStripe.PostAsync(
                    "https://api.stripe.com/v1/customers",
                    new FormUrlEncodedContent(customerData));

                var customerRespContent = await customerResp.Content.ReadAsStringAsync();
                var customerRoot = JsonDocument.Parse(customerRespContent).RootElement;

                if (!customerResp.IsSuccessStatusCode)
                {
                    string stripeErrorMsg = "Не удалось создать клиента в Stripe.";
                    if (customerRoot.TryGetProperty("error", out var errorElem) && errorElem.TryGetProperty("message", out var msgElem))
                    {
                        stripeErrorMsg = msgElem.GetString() ?? stripeErrorMsg;
                    }
                    Console.WriteLine($"Stripe API Error Creating Customer: {customerRespContent}");
                    return new CardSetupDetails { Success = false, ErrorMessage = stripeErrorMsg };
                }

                stripeCustomerId = customerRoot.GetProperty("id").GetString();
                if (string.IsNullOrEmpty(stripeCustomerId))
                {
                    return new CardSetupDetails { Success = false, ErrorMessage = "Stripe вернул пустого Customer ID." };
                }

                var newMapping = UserPaymentGatewayCustomer.Create(user.Id, provider.Id, stripeCustomerId);
                _dbContext.UserPaymentGatewayCustomers.Add(newMapping);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"Создан новый Stripe Customer: {stripeCustomerId} для пользователя {user.Id} и сохранен маппинг.");
            }

            var cardSetupRequestDto = new PrepareCardSetupRequest // Используем ваш DTO из Adapters.Payment
            {
                UserId = user.Id, // Это поле есть в PrepareCardSetupRequest вашего интерфейса IPaymentAdapter
                CustomerIdInProvider = stripeCustomerId,
                ReturnUrl = request.ReturnUrl,
                Metadata = request.Metadata
            };

            // Вызываем метод адаптера, который теперь ожидает CustomerIdInProvider
            var cardSetupDetails = await paymentAdapter.PrepareCardSetupAsync(cardSetupRequestDto);

            return cardSetupDetails;
        }
    }
}
