namespace BillingService.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public string IdType { get; private set; } = string.Empty;   // CC, NIT, PAS, etc.
        public string Document { get; private set; } = string.Empty;
        public string LegalName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public string? Phone { get; private set; }
        public string Status { get; private set; } = "ACTIVE";       // ACTIVE / INACTIVE
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Constructor
        public Customer(Guid id, string idType, string document, string legalName, string email, string address, string? phone = null)
        {
            Id = id;
            IdType = idType;
            Document = document;
            LegalName = legalName;
            Email = email;
            Address = address;
            Phone = phone;
            Status = "ACTIVE";
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

