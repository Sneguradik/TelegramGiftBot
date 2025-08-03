using System.Linq.Expressions;
using Domain.Entities.TelegramValueTransfer;

namespace Domain.Interfaces.Repos.TelegramValueTransfer;

public interface IGiftInvoiceRepo
{
    Task<GiftInvoice?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default);

    Task<IEnumerable<GiftInvoice>> GetByUserAsync(long userId, bool withTracking = false,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<GiftInvoice>> GetAllWithRecipientsAndClientsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<GiftInvoice>> GetByRecipientIdAsync(long recipientId, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<GiftInvoice>> FindAsync(Expression<Func<GiftInvoice, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default);
    
    Task AddAsync(GiftInvoice invoice, CancellationToken cancellationToken = default);
    void Update(GiftInvoice invoice);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
