using PayPal.Api;

namespace Skelbimu_sistema.Services
{
    public class PaypalServices : IPaypalServices
    {
        private readonly APIContext apiContext;
        private readonly Payment payment;
        private readonly IConfiguration configuration;

        public PaypalServices(IConfiguration configuration)
        {
            this.configuration = configuration;
            var clientId = configuration["Paypal:ClientId"];
            var clientSecret = configuration["Paypal:ClientSecret"];

            var config = new Dictionary<string, string>
            {
                { "mode", "sandbox" },
                { "clientId", clientId },
                { "clientSecret", clientSecret }
            };

            var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            apiContext = new APIContext(accessToken);

            payment = new Payment
            {
                intent = "sale",
                payer = new Payer
                {
                    payment_method = "paypal"
                }
            };
        }
        public async Task<Payment> CreateOrderAsync(decimal amount, string returnUrl, string cancelUrl)
        {
            var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPal:ClientId"], configuration["PayPal:ClientSecret"]).GetAccessToken());
            var transaction = new Transaction()
            {
                amount = new Amount()
                {
                    currency = "USD",
                    total = amount.ToString()
                },
                description = "Pardavėjo mokėstis."
            };
            var payment = new Payment()
            {
                intent = "sale",
                payer = new Payer() { payment_method = "paypal" },
                transactions = new List<Transaction>() { transaction },
                redirect_urls = new RedirectUrls()
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                }
            };
            var createdPayment = payment.Create(apiContext);
            return createdPayment;
        }
    }
}
