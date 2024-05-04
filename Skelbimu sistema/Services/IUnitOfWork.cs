namespace Skelbimu_sistema.Services
{
    public interface IUnitOfWork
    {
        IPaypalServices PaypalServices { get; }
    }
}
