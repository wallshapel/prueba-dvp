using BillingService.Application.DTOs;
using BillingService.Application.Interfaces;
using System.Text.RegularExpressions;

namespace BillingService.Application.Validators
{
    public class CreateInvoiceValidator : IInvoiceValidator
    {
        public void Validate(CreateInvoiceRequest request)
        {
            var errors = new Dictionary<string, string>();

            if (request == null)
                throw new ArgumentException("Request body cannot be null.");

            if (request.CustomerId == Guid.Empty)
                errors["customerId"] = "CustomerId is required.";

            if (string.IsNullOrWhiteSpace(request.Number))
                errors["number"] = "Invoice number is required.";

            if (request.Number?.Length > 20)
                errors["number"] = "Invoice number cannot exceed 20 characters.";

            if (request.TotalAmount <= 0)
                errors["totalAmount"] = "Total amount must be greater than zero.";

            if (string.IsNullOrWhiteSpace(request.Currency))
                errors["currency"] = "Currency is required.";

            if (request.Currency?.Length != 3 || !Regex.IsMatch(request.Currency, @"^[A-Z]{3}$"))
                errors["currency"] = "Currency must be a 3-letter ISO code (e.g. USD).";

            if (request.IssueDate == default)
                errors["issueDate"] = "Issue date is required.";

            if (request.Notes?.Length > 200)
                errors["notes"] = "Notes cannot exceed 200 characters.";

            if (errors.Any())
                throw new ArgumentException("Validation failed: " + string.Join("; ", errors.Select(e => $"{e.Key}: {e.Value}")));
        }
    }
}
