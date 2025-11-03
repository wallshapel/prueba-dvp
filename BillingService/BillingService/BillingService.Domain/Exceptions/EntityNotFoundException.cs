namespace BillingService.Domain.Exceptions
{
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string entityName, string id)
            : base($"{entityName} with ID '{id}' was not found.") { }
    }
}
