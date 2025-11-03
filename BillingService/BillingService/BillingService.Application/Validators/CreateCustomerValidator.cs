using BillingService.Application.DTOs;
using BillingService.Application.Interfaces;
using System.Text.RegularExpressions;

namespace BillingService.Application.Validators
{
    public class CreateCustomerValidator : ICustomerValidator
    {
        public void Validate(CreateCustomerRequest request)
        {
            var errors = new Dictionary<string, string>();

            if (request == null)
                throw new ArgumentException("Request body cannot be null.");

            if (string.IsNullOrWhiteSpace(request.IdType))
                errors["idType"] = "IdType is required.";

            if (string.IsNullOrWhiteSpace(request.Document))
                errors["document"] = "Document is required.";

            if (string.IsNullOrWhiteSpace(request.LegalName))
                errors["legalName"] = "Legal name is required.";

            if (string.IsNullOrWhiteSpace(request.Email))
                errors["email"] = "Email is required.";
            else if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors["email"] = "Email format is invalid.";

            if (string.IsNullOrWhiteSpace(request.Address))
                errors["address"] = "Address is required.";

            if (request.Phone?.Length > 20)
                errors["phone"] = "Phone cannot exceed 20 characters.";

            if (errors.Any())
                throw new ArgumentException("Validation failed: " + string.Join("; ", errors.Select(e => $"{e.Key}: {e.Value}")));
        }
    }
}
