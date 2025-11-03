using BillingService.Application.DTOs;

namespace BillingService.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceRequest request);
        Task<InvoiceDto> GetByIdAsync(Guid id);
        Task<IEnumerable<InvoiceDto>> GetByDateRangeAsync(DateTime from, DateTime to);
    }
}
