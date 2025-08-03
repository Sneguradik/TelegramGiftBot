using Domain.Entities.TelegramUsers;
using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Repos;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Services.Telegram;

namespace Application.Services;

public class GiftBuyingService(ITelegramGiftBuyer buyer, ITelegramClientWalletRepo walletRepo, IUnitOfWork uof) : IGiftBuyingService
{
    public async Task<GiftTransaction?> BuyGiftForRecipientAsync(Gift gift, TelegramRecipient recipient, CancellationToken cancellationToken = default)
    {
        var wallet = await walletRepo
            .GetByClientIdAsync(recipient.ClientId, true, cancellationToken);

        if (wallet == null) return null;
        
        await buyer.BuyGiftAsync(gift, recipient, cancellationToken);
        
        wallet.Amount -= gift.Price;
        
        await uof.SaveChangesAsync(cancellationToken);
        
        return new GiftTransaction
        {
            Recipient = recipient,
            GiftId = gift.Id,
            TransactionDate = DateTime.UtcNow
        };
        
    }
}