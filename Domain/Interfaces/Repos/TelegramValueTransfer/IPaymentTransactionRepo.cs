using System.Linq.Expressions;
using Domain.Entities.TelegramValueTransfer;

namespace Domain.Interfaces.Repos.TelegramValueTransfer;

public interface IPaymentTransactionRepo
{
    Task<PaymentTransaction?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentTransaction>> GetByWalletIdAsync(long walletId, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentTransaction>> FindAsync(Expression<Func<PaymentTransaction, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default);

    Task AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);
    void Update(PaymentTransaction transaction);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
