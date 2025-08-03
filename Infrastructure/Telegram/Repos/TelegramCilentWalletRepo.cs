using System.Linq.Expressions;
using Domain.Entities.TelegramUsers;
using Domain.Interfaces.Repos.TelegramUsers;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Telegram.Repos;

public class TelegramClientWalletRepo(ApplicationDbContext context) : ITelegramClientWalletRepo
{
    public async Task<TelegramClientWallet?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    public async Task<TelegramClientWallet?> GetByClientIdAsync(long clientId, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .FirstOrDefaultAsync(w => w.ClientId == clientId, cancellationToken);

    public async Task<IEnumerable<TelegramClientWallet>> FindByQueryAsync(Expression<Func<TelegramClientWallet, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).Where(predicate).ToListAsync(cancellationToken);

    public Task AddAsync(TelegramClientWallet wallet, CancellationToken cancellationToken = default)
    {
        context.TelegramClientWallets.Attach(wallet);
        return Task.CompletedTask;
    }

    public void Update(TelegramClientWallet wallet) =>
        context.TelegramClientWallets.Update(wallet);

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default) =>
        await context.TelegramClientWallets.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);

    public async Task<double> GetMaxBalanceAsync(CancellationToken cancellationToken = default) => await context
        .TelegramClientWallets
        .MaxAsync(x => x.Amount, cancellationToken: cancellationToken);

    private IQueryable<TelegramClientWallet> Query(bool withTracking) =>
        withTracking ? context.TelegramClientWallets.AsQueryable() : context.TelegramClientWallets.AsNoTracking().AsQueryable();
    
    
}
