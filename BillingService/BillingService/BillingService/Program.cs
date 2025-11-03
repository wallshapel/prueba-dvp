using BillingService.WebApi.Configurations;
using BillingService.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using BillingService.Infrastructure.Persistence.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Custom dependencies (incluye DbContext)
builder.Services.AddBillingDependencies(builder.Configuration);

var app = builder.Build();


// ✅ 1. Automatic migrations when starting the container
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();

    try
    {
        Console.WriteLine("⏳ Checking connection to Oracle...");
        db.Database.CanConnect(); // Check the connection.

        Console.WriteLine("✅ Running migrations...");
        db.Database.Migrate();
        Console.WriteLine("✅ Migrations applied correctly.");
    }
    catch (OracleException ex)
    {
        Console.WriteLine($"❌ Error connecting or migrating the database: {ex.Message}");
        throw; // Let the container fail if Oracle is not ready
    }
}


// ✅ 2. Configure Swagger in production as well (only if enabled by variable)
var enableSwagger = builder.Configuration.GetValue<bool>("EnableSwagger");
if (enableSwagger || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ✅ 3. Middleware and HTTP configuration
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
