using System.Linq.Expressions;
using Domain.Entities.TelegramUsers;

namespace Domain.Interfaces.Repos.TelegramUsers;

public interface ITelegramRecipientRepo
{
    Task<TelegramRecipient?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelegramRecipient>> GetByClientIdAsync(long clientId, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelegramRecipient>> GetByTypeAsync(RecipientType type, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelegramRecipient>> FindAsync(Expression<Func<TelegramRecipient, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default);
    
    Task AddAsync(TelegramRecipient recipient, CancellationToken cancellationToken = default);
    void Update(TelegramRecipient recipient);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}