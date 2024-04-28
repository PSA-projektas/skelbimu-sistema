using PayPal.Api;

namespace Skelbimu_sistema.Services
{
    public interface IPaypalServices
    {
        Task<Payment> CreateOrderAsync(decimal amount, string returnUrl, string cancelUrl);
    }
}
