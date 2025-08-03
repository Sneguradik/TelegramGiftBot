using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Repos;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Payments;

public class PaymentHandler(ITelegramBotClient botClient, ITelegramClientWalletRepo walletRepo, IPaymentTransactionRepo paymentTransactionRepo, IUnitOfWork uof) : IPaymentHandler<Update>
{
    public const string Name = nameof(PaymentHandler);
    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        var payment = update.Message.SuccessfulPayment;
        if (payment is null) return false;
        
        var wallet = await walletRepo.GetByClientIdAsync(update.Message.From.Id, true, cancellationToken);
        
        if (wallet is null) return false;

        var paymentTransaction = new PaymentTransaction()
        {
            Date = DateTime.UtcNow,
            WalletId = wallet.Id,
            Amount = payment.TotalAmount,
        };
        
        wallet.Amount += payment.TotalAmount;

        var priority = Math.Round(wallet.Amount / await walletRepo.GetMaxBalanceAsync(cancellationToken) * 100, 2);
        
        wallet.Client.Priority = (float)priority;
        
        await walletRepo.AddAsync(wallet, cancellationToken);
        
        await paymentTransactionRepo.AddAsync(paymentTransaction, cancellationToken);

        await uof.SaveChangesAsync(cancellationToken);
        
        
        
        return true;
    }
}