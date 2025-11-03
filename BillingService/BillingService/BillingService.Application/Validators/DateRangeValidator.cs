namespace BillingService.Application.Validators
{
    public static class DateRangeValidator
    {
        public static void Validate(DateTime from, DateTime to)
        {
            if (from == default)
                throw new ArgumentException("Start date (from) is required.");

            if (to == default)
                throw new ArgumentException("End date (to) is required.");

            if (from > to)
                throw new ArgumentException("Start date cannot be later than end date.");

            if ((to - from).TotalDays > 365)
                throw new ArgumentException("Date range cannot exceed one year.");
        }
    }
}
