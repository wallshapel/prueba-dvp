using BillingService.Application.DTOs;

namespace BillingService.Application.Interfaces
{
    public interface IInvoiceValidator
    {
        void Validate(CreateInvoiceRequest request);
    }
}
