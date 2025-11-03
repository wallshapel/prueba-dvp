namespace BillingService.Application.DTOs
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Number { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
