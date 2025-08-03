using System.ComponentModel.DataAnnotations;
using Domain.Entities.TelegramUsers;

namespace Domain.Entities.TelegramValueTransfer;

public class GiftTransaction
{
    [Key]
    public long Id { get; set; }
    public long RecipientId { get; set; }
    public TelegramRecipient Recipient { get; set; } = null!;
    public int Price { get; set; }
    public long GiftId { get; set; } 
    public DateTime TransactionDate { get; set; }
}