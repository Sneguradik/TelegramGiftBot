using System.Linq.Expressions;
using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Telegram.Repos;

public class PaymentTransactionRepo(ApplicationDbContext context) : IPaymentTransactionRepo
{
    public async Task<PaymentTransaction?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IEnumerable<PaymentTransaction>> GetByWalletIdAsync(long walletId, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).Where(t => t.WalletId == walletId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<PaymentTransaction>> FindAsync(Expression<Func<PaymentTransaction, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).Where(predicate).ToListAsync(cancellationToken);

    public Task AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
    {
        context.PaymentTransactions.AddAsync(transaction, cancellationToken);
        return Task.CompletedTask;
    }

    public void Update(PaymentTransaction transaction) =>
        context.PaymentTransactions.Update(transaction);

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default) =>
        await context.PaymentTransactions.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);

    private IQueryable<PaymentTransaction> Query(bool withTracking) =>
        withTracking ? context.PaymentTransactions : context.PaymentTransactions.AsNoTracking();
}
