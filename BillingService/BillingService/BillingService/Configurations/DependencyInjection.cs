using BillingService.Application.Interfaces;
using BillingService.Application.Services;
using BillingService.Application.Validators;
using BillingService.Domain.Interfaces;
using BillingService.Infrastructure.Clients;
using BillingService.Infrastructure.Persistence.Contexts;
using BillingService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BillingService.WebApi.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBillingDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Oracle EF Core DbContext with environment variables
            var oracleHost = Environment.GetEnvironmentVariable("ORACLE_HOST") ?? "localhost";
            var oraclePort = Environment.GetEnvironmentVariable("ORACLE_PORT") ?? "1521";
            var oracleService = Environment.GetEnvironmentVariable("ORACLE_SERVICE") ?? "XEPDB1";
            var oracleUser = Environment.GetEnvironmentVariable("ORACLE_USER") ?? "system";
            var oraclePassword = Environment.GetEnvironmentVariable("ORACLE_PASSWORD") ?? "TuContrasena123*";

            var oracleConnection = $"User Id={oracleUser};Password={oraclePassword};Data Source={oracleHost}:{oraclePort}/{oracleService}";

            services.AddDbContext<BillingDbContext>(options =>
                options.UseOracle(oracleConnection));

            // Application services
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IInvoiceService, InvoiceService>();

            // Validators
            services.AddScoped<ICustomerValidator, CreateCustomerValidator>();
            services.AddScoped<IInvoiceValidator, CreateInvoiceValidator>();

            // Domain repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            // Audit client (Rails)
            services.AddHttpClient<IAuditClient, AuditClientHttp>(client =>
            {
                var auditBaseUrl = configuration.GetValue<string>("AuditService:BaseUrl");
                client.BaseAddress = new Uri(auditBaseUrl ?? "http://127.0.0.1:3000");
                client.Timeout = TimeSpan.FromSeconds(5);
            });

            return services;
        }
    }
}
