using BillingService.Application.DTOs;
using BillingService.Application.Interfaces;
using BillingService.Application.Validators;
using BillingService.Domain.Entities;
using BillingService.Domain.Exceptions;
using BillingService.Domain.Interfaces;

namespace BillingService.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAuditClient _auditClient;
        private readonly IInvoiceValidator _validator;

        public InvoiceService(IInvoiceRepository invoiceRepository, ICustomerRepository customerRepository, IAuditClient auditClient, IInvoiceValidator validator)
        {
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _auditClient = auditClient;
            _validator = validator;
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceRequest request)
        {
            try
            {
                // Validate DTO
                _validator.Validate(request);
                // Validate customer
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId)
                    ?? throw new EntityNotFoundException(nameof(Customer), request.CustomerId.ToString());

                // Validate uniqueness of invoice number
                var existing = await _invoiceRepository.GetByNumberAsync(request.Number);
                if (existing is not null)
                    throw new DomainException("Invoice with same number already exists.");

                var invoice = new Invoice(
                    Guid.NewGuid(),
                    request.CustomerId,
                    request.Number,
                    request.TotalAmount,
                    request.Currency,
                    request.IssueDate,
                    request.Notes
                );

                await _invoiceRepository.AddAsync(invoice);

                // 🔹 Audit successful create
                await _auditClient.SendEventAsync(
                    entityType: "Invoice",
                    entityId: invoice.Id.ToString(),
                    action: "creation",
                    performedBy: "Admin",
                    details: new { number = invoice.Number, total = invoice.TotalAmount }
                );

                return MapToDto(invoice);
            }
            catch (Exception ex)
            {
                // 🔹 Audit failed create
                await _auditClient.SendEventAsync(
                    entityType: "Invoice",
                    entityId: "unknown",
                    action: "error when creating",
                    performedBy: "Admin",
                    details: new { message = ex.Message }
                );

                throw;
            }
        }

        public async Task<InvoiceDto> GetByIdAsync(Guid id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id)
                    ?? throw new EntityNotFoundException(nameof(Invoice), id.ToString());

                // 🔹 Audit successful read
                await _auditClient.SendEventAsync(
                    entityType: "Invoice",
                    entityId: invoice.Id.ToString(),
                    action: "read by id",
                    performedBy: "Admin",
                    details: new { number = invoice.Number }
                );

                return MapToDto(invoice);
            }
            catch (Exception ex)
            {
                // 🔹 Audit failed read
                await _auditClient.SendEventAsync(
                    entityType: "Invoice",
                    entityId: id.ToString(),
                    action: "error reading by id",
                    performedBy: "Admin",
                    details: new { message = ex.Message }
                );

                throw;
            }
        }

        public async Task<IEnumerable<InvoiceDto>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            
            try
            {
                //  Validate input range
                DateRangeValidator.Validate(from, to);

                var invoices = await _invoiceRepository.GetByDateRangeAsync(from, to);

                // 🔹 Audit only if there are results
                if (invoices.Any())
                {
                    await _auditClient.SendEventAsync(
                        entityType: "Invoice",
                        entityId: "range",
                        action: "read by date range",
                        performedBy: "Admin",
                        details: new { from, to, count = invoices.Count() }
                    );
                }

                return invoices.Select(MapToDto);
            }
            catch (Exception ex)
            {
                // 🔹 Audit failed read
                await _auditClient.SendEventAsync(
                    entityType: "Invoice",
                    entityId: "unknown",
                    action: "error reading by date range",
                    performedBy: "Admin",
                    details: new { message = ex.Message }
                );

                throw;
            }
           
        }

        private static InvoiceDto MapToDto(Invoice i) => new()
        {
            Id = i.Id,
            CustomerId = i.CustomerId,
            Number = i.Number,
            TotalAmount = i.TotalAmount,
            Currency = i.Currency,
            IssueDate = i.IssueDate,
            Status = i.Status,
            Notes = i.Notes,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };
    }
}
