namespace BillingService.Application.DTOs
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string IdType { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string LegalName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
