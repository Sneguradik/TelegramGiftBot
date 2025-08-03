using System.ComponentModel.DataAnnotations;
using Domain.Entities.TelegramUsers;

namespace Domain.Entities.TelegramValueTransfer;

public class PaymentTransaction
{
    [Key]
    public long Id { get; set; }
    public long WalletId { get; set; }
    public TelegramClientWallet Wallet { get; set; } = null!;
    public double Amount { get; set; }
    public DateTime Date { get; set; }
}