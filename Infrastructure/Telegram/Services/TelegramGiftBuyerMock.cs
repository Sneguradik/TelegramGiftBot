using Domain.Entities.TelegramUsers;
using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Services.Telegram;

namespace Infrastructure.Telegram.Services;

public class TelegramGiftBuyerMock : ITelegramGiftBuyer
{
    public Task InitAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Gift>> GetAvailableGiftsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Enumerable.Empty<Gift>());
    }

    public Task<GiftTransaction> BuyGiftAsync(Gift gift, TelegramRecipient recipient, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new  GiftTransaction());
    }
}