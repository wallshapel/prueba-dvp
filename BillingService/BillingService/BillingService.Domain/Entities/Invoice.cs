namespace BillingService.Domain.Entities
{
    public class Invoice
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public string Number { get; private set; } = string.Empty;
        public decimal TotalAmount { get; private set; }
        public string Currency { get; private set; } = "COP";
        public DateTime IssueDate { get; private set; }
        public string Status { get; private set; } = "ISSUED";   // ISSUED / VOIDED
        public string? Notes { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Constructor
        public Invoice(Guid id, Guid customerId, string number, decimal totalAmount, string currency, DateTime issueDate, string? notes = null)
        {
            if (totalAmount <= 0)
                throw new ArgumentException("Total amount must be greater than zero.", nameof(totalAmount));

            Id = id;
            CustomerId = customerId;
            Number = number;
            TotalAmount = totalAmount;
            Currency = currency;
            IssueDate = issueDate;
            Notes = notes;
            Status = "ISSUED";
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
