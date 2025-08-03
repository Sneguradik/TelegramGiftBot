using Domain.Entities.TelegramUsers;

namespace Domain.Entities.TelegramValueTransfer;

public class PaymentInvoice
{
    public long Id { get; set; }
    public long WalletId { get; set; }
    public TelegramClientWallet Wallet { get; set; } = null!;
    public double Amount { get; set; }
    public DateTime Created { get; set; }
}