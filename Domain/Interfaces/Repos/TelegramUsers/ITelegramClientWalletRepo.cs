using System.Linq.Expressions;
using Domain.Entities.TelegramUsers;

namespace Domain.Interfaces.Repos.TelegramUsers;

public interface ITelegramClientWalletRepo
{
    Task<TelegramClientWallet?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<TelegramClientWallet?> GetByClientIdAsync(long clientId, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelegramClientWallet>> FindByQueryAsync(Expression<Func<TelegramClientWallet, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default);
    Task AddAsync(TelegramClientWallet wallet, CancellationToken cancellationToken = default);
    void Update(TelegramClientWallet wallet);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    
    Task<double> GetMaxBalanceAsync(CancellationToken cancellationToken = default);
}

