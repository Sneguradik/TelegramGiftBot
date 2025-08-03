using System.Linq.Expressions;
using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Telegram.Repos;

public class PaymentInvoiceRepo(ApplicationDbContext context) : IPaymentInvoiceRepo
{
    public async Task<PaymentInvoice?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IEnumerable<PaymentInvoice>> GetByWalletIdAsync(long walletId, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).Where(i => i.WalletId == walletId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<PaymentInvoice>> FindAsync(Expression<Func<PaymentInvoice, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).Where(predicate).ToListAsync(cancellationToken);

    public Task AddAsync(PaymentInvoice invoice, CancellationToken cancellationToken = default)
    {
        context.PaymentInvoices.Attach(invoice);
        return Task.CompletedTask;
    }
        

    public void Update(PaymentInvoice invoice) =>
        context.PaymentInvoices.Update(invoice);

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default) =>
        await context.PaymentInvoices.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);

    private IQueryable<PaymentInvoice> Query(bool withTracking) =>
        withTracking ? context.PaymentInvoices : context.PaymentInvoices.AsNoTracking();
}
