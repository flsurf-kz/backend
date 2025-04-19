using System.Net.Http.Headers;
using System.Text.Json;

namespace Flsurf.Infrastructure.Adapters.Payment.Systems
{
    public class StripeConfig
    {
        public string SecretKey { get; set; } = null!;

    }

    public class StripePaymentAdapter : IPaymentAdapter
    {
        private readonly HttpClient _http;
        private readonly StripeConfig _cfg;

        public StripePaymentAdapter(HttpClient http, StripeConfig cfg)
        {
            _http = http;
            _cfg = cfg;
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", cfg.SecretKey);
        }

        public async Task<InitPaymentResult> InitPayment(PaymentInitRequest r)
        {
            var data = new Dictionary<string, string>
            {
                ["amount"] = ((int)(r.Amount * 100)).ToString(),
                ["currency"] = r.Currency.ToLower(),
                ["metadata[order_id]"] = r.OrderId,
                ["description"] = r.Description
            };

            if (!string.IsNullOrEmpty(r.ProviderPaymentMethodToken))
            {
                // token charge
                data["payment_method"] = r.ProviderPaymentMethodToken;
                data["confirmation_method"] = "automatic";
                data["confirm"] = "true";
            }
            else
            {
                // redirect‑checkout
                data["payment_method_types[]"] = "card";
                data["return_url"] = r.SuccessReturnUrl;
                data["confirmation_method"] = "manual";
            }

            var resp = await _http.PostAsync(
                "https://api.stripe.com/v1/payment_intents",
                new FormUrlEncodedContent(data));

            var root = JsonDocument.Parse(await resp.Content.ReadAsStringAsync()).RootElement;
            var id = root.GetProperty("id").GetString()!;
            var ok = resp.IsSuccessStatusCode;

            string? redirect = null;
            if (root.TryGetProperty("next_action", out var act) &&
                act.GetProperty("type").GetString() == "redirect_to_url")
                redirect = act.GetProperty("redirect_to_url").GetProperty("url").GetString();

            return new InitPaymentResult(
                ok, id,
                RedirectUrl: redirect,
                QrUrl: null,
                ClientSecret: root.GetProperty("client_secret").GetString());
        }

        public async Task<CardMeta?> FetchCardMetaAsync(string token)
        {
            var r = await _http.GetAsync($"https://api.stripe.com/v1/payment_methods/{token}");
            if (!r.IsSuccessStatusCode) return null;

            var card = JsonDocument.Parse(await r.Content.ReadAsStringAsync())
                                    .RootElement.GetProperty("card");
            return new CardMeta(
                card.GetProperty("brand").GetString()!,
                card.GetProperty("last4").GetString()!,
                card.GetProperty("exp_month").GetInt32(),
                card.GetProperty("exp_year").GetInt32());
        }

        public async Task<PaymentStatus> GetStatusAsync(string pid)
        {
            var r = await _http.GetAsync($"https://api.stripe.com/v1/payment_intents/{pid}");
            var st = JsonDocument.Parse(await r.Content.ReadAsStringAsync())
                                    .RootElement.GetProperty("status").GetString();
            return st switch
            {
                "succeeded" => PaymentStatus.Completed,
                "processing" => PaymentStatus.Pending,
                _ => PaymentStatus.Failed
            };
        }

        public async Task<RefundResult> RefundAsync(string pid, decimal amt)
        {
            var body = new Dictionary<string, string>
            {
                ["payment_intent"] = pid,
                ["amount"] = ((int)(amt * 100)).ToString()
            };
            var r = await _http.PostAsync("https://api.stripe.com/v1/refunds",
                                            new FormUrlEncodedContent(body));
            return new RefundResult(r.IsSuccessStatusCode,
                await r.Content.ReadAsStringAsync());
        }
    }
}
