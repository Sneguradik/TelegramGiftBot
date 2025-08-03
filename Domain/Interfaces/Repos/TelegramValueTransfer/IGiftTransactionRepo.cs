using System.Linq.Expressions;
using Domain.Entities.TelegramValueTransfer;

namespace Domain.Interfaces.Repos.TelegramValueTransfer;

public interface IGiftTransactionRepo
{
    Task<GiftTransaction?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<GiftTransaction>> GetByRecipientIdAsync(long recipientId, bool withTracking = false, CancellationToken cancellationToken = default);

    Task<IEnumerable<GiftTransaction>> GetByUserAsync(long userId, bool withTracking = false,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<GiftTransaction>> FindAsync(Expression<Func<GiftTransaction, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default);
    
    Task AddAsync(GiftTransaction transaction, CancellationToken cancellationToken = default);
    void Update(GiftTransaction transaction);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}