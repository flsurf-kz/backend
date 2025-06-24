using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Flsurf.Infrastructure.Adapters.Payment.Systems
{
    public class StripeConfig
    {
        public string SecretKey { get; set; } = null!;

    }

    // Вспомогательные классы для парсинга ответов Stripe (если не используете Stripe.net DTO)
    // Вы можете создать более строгие DTO или использовать JsonNode/JsonElement как в вашем коде.
    internal class StripeSetupIntentResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("client_secret")]
        public string? ClientSecret { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("customer")]
        public string? Customer { get; set; } // ID клиента Stripe
    }

    internal class StripeCustomerResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        // другие поля Customer при необходимости
    }


    public class StripePaymentAdapter : IPaymentAdapter
    {
        private readonly HttpClient _http;
        private readonly StripeConfig _cfg;
        private readonly ILogger _logger;   
        
        public StripePaymentAdapter(HttpClient http, StripeConfig cfg, ILogger<StripePaymentAdapter> logger)
        {
            _logger = logger;
            _http = http;
            _cfg = cfg;
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", cfg.SecretKey);
        }

        // --- Существующие методы (FetchCardMetaAsync, InitPayment, и т.д.) остаются как есть ---
        public async Task<CardMeta?> FetchCardMetaAsync(string token)
        {
            var r = await _http.GetAsync($"https://api.stripe.com/v1/payment_methods/{token}");
            if (!r.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Stripe API Error FetchCardMetaAsync: {await r.Content.ReadAsStringAsync()}, StatusCode: {r.StatusCode}");
                return null;
            }

            var body = await r.Content.ReadAsStringAsync(); 
            var jsonDoc = JsonDocument.Parse(body);
            if (!jsonDoc.RootElement.TryGetProperty("card", out var cardElement))
            {
                _logger.LogInformation($"Stripe API Error FetchCardMetaAsync: 'card' property not found in response for token {token}, body: {jsonDoc.RootElement.ToString()}");
                return null;
            }
            
            _logger.LogInformation("Stripe card metadata checked.");

            return new CardMeta(
                cardElement.TryGetProperty("brand", out var brand) ? brand.GetString()! : "unknown",
                cardElement.TryGetProperty("last4", out var last4) ? last4.GetString()! : "****",
                cardElement.TryGetProperty("exp_month", out var expMonth) ? expMonth.GetInt32() : 0,
                cardElement.TryGetProperty("exp_year", out var expYear) ? expYear.GetInt32() : 0
            );
        }

        // ... ваш существующий InitPayment, GetStatusAsync, RefundAsync ...
        // Пример вашего InitPayment, оставлен для контекста
        public async Task<InitPaymentResult> InitPayment(PaymentInitRequest r)
        {
            var data = new Dictionary<string, string>
            {
                ["amount"]      = ((long)(r.Amount * 100)).ToString(),
                ["currency"]    = r.Currency.ToLower(),
                ["metadata[order_id]"] = r.OrderId,
                ["description"] = r.Description
            };

            if (!string.IsNullOrEmpty(r.ProviderPaymentMethodToken))
            {
                /* платёж через сохранённую карту ― сразу confirm */
                data["payment_method"]      = r.ProviderPaymentMethodToken;
                data["confirmation_method"] = "automatic";
                data["confirm"]             = "true";
                data["return_url"]          = r.SuccessReturnUrl;      // теперь допустимо
            }
            else
            {
                /* Payment Element: только client_secret для фронта */
                data["automatic_payment_methods[enabled]"] = "true";
                /* return_url НЕЛЬЗЯ добавлять без confirm=true  */
            }

            var resp = await _http.PostAsync(
                "https://api.stripe.com/v1/payment_intents",
                new FormUrlEncodedContent(data));

            var body  = await resp.Content.ReadAsStringAsync();
            var root  = JsonDocument.Parse(body).RootElement;

            if (!resp.IsSuccessStatusCode)
            {
                var msg = root.TryGetProperty("error", out var e) &&
                          e.TryGetProperty("message", out var m)
                                ? m.GetString()
                                : "Stripe error";
                var errId = root.TryGetProperty("id", out var idEl)
                                ? idEl.GetString()
                                : string.Empty;

                return new InitPaymentResult(Success: false, ProviderPaymentId: errId!, ErrorMessage: msg, RedirectUrl: null, QrUrl: null, ClientSecret: "");
            }

            var id           = root.GetProperty("id").GetString()!;
            var clientSecret = root.GetProperty("client_secret").GetString()!;

            string? redirect = null;
            if (root.TryGetProperty("next_action", out var na) &&
                na.ValueKind == JsonValueKind.Object &&                // ← важно!
                na.TryGetProperty("type", out var t) &&
                t.ValueEquals("redirect_to_url") &&                    // .NET 8+
                na.TryGetProperty("redirect_to_url", out var ru) &&
                ru.TryGetProperty("url", out var u))
            {
                redirect = u.GetString();
            }

            return new InitPaymentResult(
                Success: true, ProviderPaymentId: id, ErrorMessage: null, RedirectUrl: redirect, ClientSecret: clientSecret, QrUrl: "");
        }
        
        /// <summary>Переводим средства и инициируем payout.</summary>
        public async Task<PayoutInitResult> InitPayoutAsync(PayoutInitRequest r)
        {
            /*-----------------------------------------------------------*
             * 1) Transfer с платформы на connected-account              *
             *-----------------------------------------------------------*/
            var transferBody = new Dictionary<string, string>
            {
                ["amount"]      = ((long)(r.Amount * 100)).ToString(),
                ["currency"]    = r.CurrencyIso.ToLower(),
                ["destination"] = r.ConnectedAccountId,
                ["description"] = r.Description
            };

            var trResp = await _http.PostAsync(
                "https://api.stripe.com/v1/transfers",
                new FormUrlEncodedContent(transferBody));

            if (!trResp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Stripe transfer error: {body}",
                    await trResp.Content.ReadAsStringAsync());
                return new(false, "", "Transfer failed");
            }

            /*-----------------------------------------------------------*
             * 2) Payout из connected-аккаунта                           *
             *    NB: header  Stripe-Account: acct_…                     *
             *-----------------------------------------------------------*/
            using var req = new HttpRequestMessage(HttpMethod.Post,
                "https://api.stripe.com/v1/payouts")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["amount"]   = ((long)(r.Amount * 100)).ToString(),
                    ["currency"] = r.CurrencyIso.ToLower(),
                    ["method"]   = "instant",             // или  "standard"
                    ["destination"] = r.ExternalAccountId // card_…/ba_…
                })
            };
            req.Headers.Add("Stripe-Account", r.ConnectedAccountId);

            var poResp = await _http.SendAsync(req);
            var poBody = await poResp.Content.ReadAsStringAsync();

            if (!poResp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Stripe payout error: {body}", poBody);
                return new(false, "", "Payout failed");
            }

            var payoutId = JsonDocument.Parse(poBody)
                         .RootElement.GetProperty("id").GetString()!; // po_…

            return new(true, payoutId, null);
        }


        // НОВЫЙ МЕТОД - РЕАЛИЗАЦИЯ PrepareCardSetupAsync
        public async Task<CardSetupDetails> PrepareCardSetupAsync(PrepareCardSetupRequest request)
        {
            try
            {
                string? stripeCustomerId = request.CustomerIdInProvider;

                // Шаг 1: Убедиться, что у пользователя есть Stripe Customer ID
                // Эту логику нужно будет адаптировать под вашу систему хранения UserEntity и StripeCustomerId
                if (string.IsNullOrEmpty(stripeCustomerId))
                {
                    // ЗАГЛУШКА: Здесь должна быть логика получения UserEntity по request.UserId,
                    // проверки наличия UserEntity.StripeCustomerId, и его создания в Stripe, если нет.
                    // var user = await _dbContext.Users.FindAsync(request.UserId);
                    // if (user == null) return new CardSetupDetails { Success = false, ErrorMessage = "Пользователь не найден." };
                    // stripeCustomerId = user.StripeCustomerId; // Предполагаемое поле

                    if (string.IsNullOrEmpty(stripeCustomerId)) // Если все еще пуст после проверки в БД
                    {
                        // Создаем нового Stripe Customer
                        var customerData = new Dictionary<string, string>
                        {
                            // ["email"] = user.Email, // Пример
                            // ["name"] = user.Fullname, // Пример
                            ["metadata[flsurf_user_id]"] = request.UserId.ToString()
                        };
                        var customerResp = await _http.PostAsync(
                            "https://api.stripe.com/v1/customers",
                            new FormUrlEncodedContent(customerData));

                        var customerRespContent = await customerResp.Content.ReadAsStringAsync();
                        var customerRoot = JsonDocument.Parse(customerRespContent).RootElement;

                        if (!customerResp.IsSuccessStatusCode)
                        {
                            _logger.LogInformation($"Stripe API Error Creating Customer: {customerRespContent}");
                            return new CardSetupDetails { Success = false, ErrorMessage = "Не удалось создать клиента в Stripe." };
                        }
                        stripeCustomerId = customerRoot.GetProperty("id").GetString();
                        // Сохраните этот stripeCustomerId для вашего UserEntity в БД!
                        // user.StripeCustomerId = stripeCustomerId;
                        // await _dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Created new Stripe Customer: {stripeCustomerId} for user {request.UserId}");
                    }
                }

                // Шаг 2: Создание Stripe SetupIntent
                var setupIntentData = new Dictionary<string, string>
                {
                    ["customer"] = stripeCustomerId!,
                    // ["automatic_payment_methods[enabled]"] = "true",
                    // ["usage"] = "on_session", // или "off_session"
                    ["payment_method_types[]"] = "card", // Можно явно указать, но automatic_payment_methods обычно достаточно
                };
                if (request.Metadata != null)
                {
                    foreach (var meta in request.Metadata)
                    {
                        setupIntentData[$"metadata[{meta.Key}]"] = meta.Value;
                    }
                }


                var siResp = await _http.PostAsync(
                    "https://api.stripe.com/v1/setup_intents",
                    new FormUrlEncodedContent(setupIntentData));

                var siRespContent = await siResp.Content.ReadAsStringAsync();
                var siRoot = JsonDocument.Parse(siRespContent).RootElement;

                if (!siResp.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Stripe API Error Creating SetupIntent: {siRespContent}");
                    string errorMessage = "Ошибка при создании SetupIntent в Stripe.";
                    if (siRoot.TryGetProperty("error", out var errorElement) && errorElement.TryGetProperty("message", out var msgElement))
                    {
                        errorMessage = msgElement.GetString() ?? errorMessage;
                    }
                    return new CardSetupDetails { Success = false, ErrorMessage = errorMessage };
                }

                return new CardSetupDetails
                {
                    Success = true,
                    ProviderSetupId = siRoot.GetProperty("id").GetString()!,         // "seti_..."
                    ClientSecretForWidget = siRoot.GetProperty("client_secret").GetString()!,
                    // RedirectUrlForProviderPage обычно не используется с PaymentElement для setup,
                    // но return_url в confirmSetup на фронте важен для 3DS.
                };
            }
            catch (JsonException jsonEx)
            {
                _logger.LogInformation($"JSON Parsing Error in PrepareCardSetupAsync: {jsonEx.Message}");
                return new CardSetupDetails { Success = false, ErrorMessage = "Ошибка обработки ответа от Stripe." };
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogInformation($"HTTP Request Error in PrepareCardSetupAsync: {httpEx.Message}");
                return new CardSetupDetails { Success = false, ErrorMessage = "Ошибка сети при обращении к Stripe." };
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Generic Error in PrepareCardSetupAsync: {e.ToString()}"); // Логируем полный стектрейс
                return new CardSetupDetails { Success = false, ErrorMessage = "Внутренняя ошибка сервера при подготовке настройки карты." };
            }
        }

        // Ваши существующие GetStatusAsync и RefundAsync
        public async Task<PaymentStatus> GetStatusAsync(string pid)
        {
            // ... (ваш код, возможно, потребуется доработка для различения PaymentIntent и SetupIntent)
            // Если pid может быть SetupIntent ID (seti_...)
            string endpoint = pid.StartsWith("seti_")
                ? $"https://api.stripe.com/v1/setup_intents/{pid}"
                : $"https://api.stripe.com/v1/payment_intents/{pid}";

            var r = await _http.GetAsync(endpoint);
            var responseContent = await r.Content.ReadAsStringAsync();
            var root = JsonDocument.Parse(responseContent).RootElement;

            if (!r.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Stripe API Error GetStatusAsync for {pid}: {responseContent}");
                return PaymentStatus.Failed;
            }

            var st = root.GetProperty("status").GetString();

            if (pid.StartsWith("seti_"))
            {
                return st switch
                {
                    "succeeded" => PaymentStatus.Completed, // Карта успешно настроена
                    "requires_payment_method" => PaymentStatus.Failed,
                    "requires_confirmation" => PaymentStatus.Pending,
                    "requires_action" => PaymentStatus.Pending,
                    "processing" => PaymentStatus.Pending,
                    "canceled" => PaymentStatus.Failed, // или свой статус Canceled
                    _ => PaymentStatus.Pending
                };
            }
            else
            { // payment_intent
                return st switch
                {
                    "succeeded" => PaymentStatus.Completed,
                    "processing" => PaymentStatus.Pending,
                    "requires_payment_method" => PaymentStatus.Failed,
                    "requires_confirmation" => PaymentStatus.Pending,
                    "requires_action" => PaymentStatus.Pending,
                    "canceled" => PaymentStatus.Failed,
                    _ => PaymentStatus.Failed
                };
            }
        }

        public async Task<RefundResult> RefundAsync(string pid, decimal amt)
        {
            var body = new Dictionary<string, string>
            {
                ["payment_intent"] = pid, // Убедитесь, что это PaymentIntent ID
                ["amount"] = ((int)(amt * 100)).ToString()
            };
            var r = await _http.PostAsync("https://api.stripe.com/v1/refunds",
                                          new FormUrlEncodedContent(body));
            var responseContent = await r.Content.ReadAsStringAsync();
            if (!r.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Stripe API Error RefundAsync: {responseContent}");
                // Попытка извлечь сообщение об ошибке из ответа Stripe
                string errorMessage = "Ошибка при возврате.";
                try
                {
                    var root = JsonDocument.Parse(responseContent).RootElement;
                    if (root.TryGetProperty("error", out var errorElement) && errorElement.TryGetProperty("message", out var msgElement))
                    {
                        errorMessage = msgElement.GetString() ?? errorMessage;
                    }
                }
                catch { /* игнорируем ошибку парсинга, если ответ не JSON */ }
                return new RefundResult(false, errorMessage);
            }
            return new RefundResult(r.IsSuccessStatusCode, responseContent);
        }
    }
}
