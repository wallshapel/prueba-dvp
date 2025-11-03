using BillingService.Domain.Entities;

namespace BillingService.Domain.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<Invoice?> GetByNumberAsync(string number);
        Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to);
    }
}
