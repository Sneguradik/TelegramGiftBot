using System.Linq.Expressions;
using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Telegram.Repos;

public class GiftTransactionRepo(ApplicationDbContext context) : IGiftTransactionRepo
{
    public async Task<GiftTransaction?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IEnumerable<GiftTransaction>> GetByRecipientIdAsync(long recipientId, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .Where(t => t.RecipientId == recipientId)
            .AsSplitQuery()
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    
    public async Task<IEnumerable<GiftTransaction>> GetByUserAsync(long userId,bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .AsSplitQuery()
            .Where(t => t.Recipient.Client.Id == userId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<GiftTransaction>> FindAsync(Expression<Func<GiftTransaction, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .Where(predicate)
            .ToListAsync(cancellationToken);

    public Task AddAsync(GiftTransaction transaction, CancellationToken cancellationToken = default)
    {
        context.GiftTransactions.Attach(transaction);
        return Task.CompletedTask;
    }
         

    public void Update(GiftTransaction transaction) =>
        context.GiftTransactions
            .Update(transaction);

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default) =>
        await context.GiftTransactions
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

    private IQueryable<GiftTransaction> Query(bool withTracking) =>
        withTracking ? context.GiftTransactions : context.GiftTransactions.AsNoTracking();
}
