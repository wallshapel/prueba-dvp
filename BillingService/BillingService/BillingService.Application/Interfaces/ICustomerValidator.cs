using BillingService.Application.DTOs;

namespace BillingService.Application.Interfaces
{
    public interface ICustomerValidator
    {
        void Validate(CreateCustomerRequest request);
    }
}
