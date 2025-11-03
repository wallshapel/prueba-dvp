using BillingService.Domain.Entities;

namespace BillingService.Domain.Tests.Entities
{
    public class InvoiceTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            // Act
            var invoice = new Invoice(
                id,
                customerId,
                "INV-001",
                1000.50m,
                "USD",
                new DateTime(2025, 10, 31),
                "Test invoice"
            );

            // Assert
            Assert.Equal(id, invoice.Id);
            Assert.Equal(customerId, invoice.CustomerId);
            Assert.Equal("INV-001", invoice.Number);
            Assert.Equal(1000.50m, invoice.TotalAmount);
            Assert.Equal("USD", invoice.Currency);
            Assert.Equal(new DateTime(2025, 10, 31), invoice.IssueDate);
            Assert.Equal("Test invoice", invoice.Notes);
            Assert.Equal("ISSUED", invoice.Status);
            Assert.True(invoice.CreatedAt <= DateTime.UtcNow);
            Assert.True(invoice.UpdatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenTotalAmountIsZeroOrNegative()
        {
            // Arrange
            var id = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new Invoice(id, customerId, "INV-002", 0, "USD", DateTime.UtcNow)
            );

            Assert.Equal("Total amount must be greater than zero. (Parameter 'totalAmount')", ex.Message);
        }

        [Fact]
        public void Constructor_ShouldAllowOptionalNotesParameter()
        {
            var id = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var invoice = new Invoice(id, customerId, "INV-003", 500, "EUR", DateTime.UtcNow);

            Assert.Null(invoice.Notes);
        }
    }
}
