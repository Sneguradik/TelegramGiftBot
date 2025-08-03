using System.Linq.Expressions;
using Domain.Entities.TelegramValueTransfer;

namespace Domain.Interfaces.Repos.TelegramValueTransfer;

public interface IPaymentInvoiceRepo
{
    Task<PaymentInvoice?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentInvoice>> GetByWalletIdAsync(long walletId, bool withTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentInvoice>> FindAsync(Expression<Func<PaymentInvoice, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default);

    Task AddAsync(PaymentInvoice invoice, CancellationToken cancellationToken = default);
    void Update(PaymentInvoice invoice);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
