using BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Infrastructure.Persistence.Contexts
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Invoice> Invoices => Set<Invoice>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Table mappings
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMERS");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID").HasMaxLength(36);
                entity.Property(e => e.IdType).HasColumnName("ID_TYPE").HasMaxLength(10);
                entity.Property(e => e.Document).HasColumnName("DOCUMENT").HasMaxLength(30);
                entity.Property(e => e.LegalName).HasColumnName("LEGAL_NAME").HasMaxLength(100);
                entity.Property(e => e.Email).HasColumnName("EMAIL").HasMaxLength(100);
                entity.Property(e => e.Address).HasColumnName("ADDRESS").HasMaxLength(150);
                entity.Property(e => e.Phone).HasColumnName("PHONE").HasMaxLength(20);
                entity.Property(e => e.Status).HasColumnName("STATUS").HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(e => e.UpdatedAt).HasColumnName("UPDATED_AT");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("INVOICES");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID").HasMaxLength(36);
                entity.Property(e => e.CustomerId).HasColumnName("CUSTOMER_ID").HasMaxLength(36);
                entity.Property(e => e.Number).HasColumnName("NUMBER").HasMaxLength(20);
                entity.Property(e => e.TotalAmount).HasColumnName("TOTAL_AMOUNT").HasColumnType("NUMBER(12,2)");
                entity.Property(e => e.Currency).HasColumnName("CURRENCY").HasMaxLength(10);
                entity.Property(e => e.IssueDate).HasColumnName("ISSUE_DATE");
                entity.Property(e => e.Status).HasColumnName("STATUS").HasMaxLength(20);
                entity.Property(e => e.Notes).HasColumnName("NOTES").HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(e => e.UpdatedAt).HasColumnName("UPDATED_AT");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
