using System.Text.Json;
using Application.Services.Telegram.Payments;
using Domain.Constants;
using Domain.Entities;
using Domain.Entities.TelegramUsers;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Services.Telegram;
using Telegram.BotAPI;
using Telegram.BotAPI.Payments;

namespace Application.Services.Telegram;



public class StarInvoiceService(ITelegramBotClient botClient, ITelegramClientWalletRepo walletRepo) : IStarInvoiceService
{
    public async Task CreateInvoice(int amount, TelegramClient client, CancellationToken ct = default)
    {
        var wallet = await walletRepo
            .GetByClientIdAsync(client.Id, false, ct);
        if (wallet is null) return;
        
        var invoicePayload = new PaymentData()
        {
            Name = PaymentHandler.Name,
            WalletId = wallet.Id,
            Amount = amount,
            Currency = CurrencyConstants.Stars,
            CreatedAt = DateTime.UtcNow,
        };
        await botClient.SendInvoiceAsync(
            new SendInvoiceArgs(
                client.Id, 
                "Пополнение баланса", 
                "Вы пополняете баланс для отправки подарков другим пользователям Telegram. Звёзды будут зачислены на ваш счёт сразу после оплаты.", 
                JsonSerializer.Serialize(invoicePayload), 
                CurrencyConstants.Stars, 
                new []{new LabeledPrice("Balance top up", amount)}), 
            cancellationToken: ct);
    }
}