using BillingService.Domain.Entities;
using BillingService.Domain.Interfaces;
using BillingService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Infrastructure.Persistence.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly BillingDbContext _context;

        public InvoiceRepository(BillingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Invoice entity)
        {
            await _context.Invoices.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Invoices.FindAsync(id);
            if (entity != null)
            {
                _context.Invoices.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.AsNoTracking().ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(Guid id)
        {
            return await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoice?> GetByNumberAsync(string number)
        {
            return await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Number == number);
        }

        public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Where(i => i.IssueDate >= from && i.IssueDate <= to)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> FindAsync(System.Linq.Expressions.Expression<Func<Invoice, bool>> predicate)
        {
            return await _context.Invoices.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task UpdateAsync(Invoice entity)
        {
            _context.Invoices.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
