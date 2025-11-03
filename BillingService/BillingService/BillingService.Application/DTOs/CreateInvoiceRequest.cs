namespace BillingService.Application.DTOs
{
    public class CreateInvoiceRequest
    {
        public Guid CustomerId { get; set; }
        public string Number { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "COP";
        public DateTime IssueDate { get; set; }
        public string? Notes { get; set; }
    }
}
