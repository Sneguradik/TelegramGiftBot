using Domain.Entities.TelegramUsers;
using Domain.Entities.TelegramValueTransfer;

namespace Domain.Interfaces.Services.Telegram;

public interface IGiftBuyingService
{
    Task<GiftTransaction?> BuyGiftForRecipientAsync(Gift gift, TelegramRecipient recipient,
        CancellationToken cancellationToken = default);
}