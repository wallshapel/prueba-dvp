namespace BillingService.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
