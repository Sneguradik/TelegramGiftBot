using System.Linq.Expressions;
using Domain.Entities.TelegramUsers;

namespace Domain.Interfaces.Repos.TelegramUsers;

public interface ITelegramClientRepo
{
    Task<bool> CheckByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<TelegramClient?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelegramClient>> FindAsync(Expression<Func<TelegramClient, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default);
    
    Task AddAsync(TelegramClient client, CancellationToken cancellationToken = default);
    void Update(TelegramClient client);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
