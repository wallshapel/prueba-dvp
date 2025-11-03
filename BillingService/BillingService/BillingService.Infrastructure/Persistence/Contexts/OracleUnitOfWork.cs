using System.Data;
using BillingService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace BillingService.Infrastructure.Persistence.Contexts
{
    public class OracleUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly OracleConnection _connection;
        private OracleTransaction? _transaction;
        private readonly ILogger<OracleUnitOfWork> _logger;

        public IDbConnection Connection => _connection;
        public IDbTransaction? Transaction => _transaction;

        public OracleUnitOfWork(string connectionString, ILogger<OracleUnitOfWork> logger)
        {
            _connection = new OracleConnection(connectionString);
            _logger = logger;
        }

        public async Task BeginAsync()
        {
            if (_connection.State != ConnectionState.Open)
                await _connection.OpenAsync();

            _transaction = _connection.BeginTransaction();
            _logger.LogInformation("Oracle transaction started.");
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
                return;

            await Task.Run(() => _transaction.Commit());
            _logger.LogInformation("Oracle transaction committed.");
            _transaction.Dispose();
            _transaction = null;
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
                return;

            await Task.Run(() => _transaction.Rollback());
            _logger.LogWarning("Oracle transaction rolled back.");
            _transaction.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection.Dispose();
        }
    }
}
