using Domain.Entities.TelegramUsers;
using Domain.Entities.TelegramValueTransfer;

namespace Domain.Interfaces.Services.Telegram;

public interface ITelegramGiftBuyer
{
    Task InitAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Gift>> GetAvailableGiftsAsync(CancellationToken cancellationToken = default);
    
    Task<GiftTransaction> BuyGiftAsync(Gift gift, TelegramRecipient recipient, CancellationToken cancellationToken = default);   
};