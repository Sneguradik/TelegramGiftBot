using Domain.Entities.TelegramUsers;

namespace Domain.Entities.TelegramValueTransfer;

public class GiftInvoice
{
    public long Id { get; set; }
    public long RecipientId { get; set; }
    public TelegramRecipient Recipient { get; set; } = null!;
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    public int Amount { get; set; }
    public DateTime Created { get; set; }
}