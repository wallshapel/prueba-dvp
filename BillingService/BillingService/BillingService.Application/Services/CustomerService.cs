using BillingService.Application.DTOs;
using BillingService.Application.Interfaces;
using BillingService.Domain.Entities;
using BillingService.Domain.Exceptions;
using BillingService.Domain.Interfaces;

namespace BillingService.Application.Services
{
    public class CustomerService : ICustomerService 
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAuditClient _auditClient;
        private readonly ICustomerValidator _validator;

        public CustomerService(ICustomerRepository customerRepository, IAuditClient auditClient, ICustomerValidator validator)
        {
            _customerRepository = customerRepository;
            _auditClient = auditClient;
            _validator = validator;
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {
                // Validate DTO
                _validator.Validate(request);
                // Check if customer already exists
                var existing = await _customerRepository.GetByDocumentAsync(request.IdType, request.Document);
                if (existing is not null)
                    throw new DomainException("Customer with same document already exists.");

                var customer = new Customer(
                    Guid.NewGuid(),
                    request.IdType,
                    request.Document,
                    request.LegalName,
                    request.Email,
                    request.Address,
                    request.Phone
                );

                await _customerRepository.AddAsync(customer);

                // 🔹 Audit create
                await _auditClient.SendEventAsync(
                    entityType: "Customer",
                    entityId: customer.Id.ToString(),
                    action: "creation",
                    performedBy: "Admin",
                    details: new { document = customer.Document, name = customer.LegalName }
                );

                return MapToDto(customer);
            }
            catch (Exception ex)
            {
                // 🔹 Audit error on creation
                await _auditClient.SendEventAsync(
                    entityType: "Customer",
                    entityId: "unknown",
                    action: "error when creating",
                    performedBy: "Admin",
                    details: new { message = ex.Message }
                );

                throw;
            }
        }

        public async Task<CustomerDto> GetByIdAsync(Guid id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id)
                    ?? throw new EntityNotFoundException(nameof(Customer), id.ToString());

                // 🔹 Audit successful read
                await _auditClient.SendEventAsync(
                    entityType: "Customer",
                    entityId: customer.Id.ToString(),
                    action: "read by id",
                    performedBy: "Admin",
                    details: new { name = customer.LegalName }
                );

                return MapToDto(customer);
            }
            catch (Exception ex)
            {
                // 🔹 Audit error read
                await _auditClient.SendEventAsync(
                    entityType: "Customer",
                    entityId: id.ToString(),
                    action: "error reading by id",
                    performedBy: "Admin",
                    details: new { message = ex.Message }
                );

                throw;
            }
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var list = customers.Select(MapToDto).ToList();

                // 🔹 Audit successful read-all
                await _auditClient.SendEventAsync(
                    entityType: "Customer",
                    entityId: "all",
                    action: "read all",
                    performedBy: "Admin",
                    details: new { count = list.Count }
                );

                return list;
            }
            catch (Exception ex)
            {
                // 🔹 Audit error on read-all
                await _auditClient.SendEventAsync(
                    entityType: "Customer",
                    entityId: "all",
                    action: "error reading all",
                    performedBy: "Admin",
                    details: new { message = ex.Message }
                );

                throw;
            }
        }

        private static CustomerDto MapToDto(Customer c) => new()
        {
            Id = c.Id,
            IdType = c.IdType,
            Document = c.Document,
            LegalName = c.LegalName,
            Email = c.Email,
            Address = c.Address,
            Phone = c.Phone,
            Status = c.Status,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }
}
