using BillingService.Domain.Entities;

namespace BillingService.Domain.Tests.Entities
{
    public class CustomerTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var customer = new Customer(
                id,
                "CC",
                "123456789",
                "John Doe",
                "john@example.com",
                "742 Evergreen Terrace",
                "555-1111"
            );

            // Assert
            Assert.Equal(id, customer.Id);
            Assert.Equal("CC", customer.IdType);
            Assert.Equal("123456789", customer.Document);
            Assert.Equal("John Doe", customer.LegalName);
            Assert.Equal("john@example.com", customer.Email);
            Assert.Equal("742 Evergreen Terrace", customer.Address);
            Assert.Equal("555-1111", customer.Phone);
            Assert.Equal("ACTIVE", customer.Status);
            Assert.True(customer.CreatedAt <= DateTime.UtcNow);
            Assert.True(customer.UpdatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Constructor_ShouldDefaultStatusToActive_WhenCreated()
        {
            // Arrange & Act
            var customer = new Customer(
                Guid.NewGuid(),
                "NIT",
                "900999999",
                "ACME Ltda",
                "contact@acme.com",
                "123 Industrial Rd"
            );

            // Assert
            Assert.Equal("ACTIVE", customer.Status);
        }
    }
}
