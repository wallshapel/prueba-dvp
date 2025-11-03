using BillingService.Domain.Entities;

namespace BillingService.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByDocumentAsync(string idType, string document);
    }
}
