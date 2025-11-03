namespace BillingService.Application.DTOs
{
    public class CreateCustomerRequest
    {
        public string IdType { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string LegalName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}
