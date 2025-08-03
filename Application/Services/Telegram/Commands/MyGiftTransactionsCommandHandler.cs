using System.Text;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Commands;

public class MyGiftTransactionsCommandHandler(
    IGiftTransactionRepo transactionRepo, 
    ITelegramBotClient botClient
) : ICommandMessageHandler<Update>
{
    public const string CommandName = "myGiftTransactions";

    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        var userId = update.Message!.From!.Id;
        var transactions = await transactionRepo.GetByUserAsync(userId, false, cancellationToken);

        if (!transactions.Any())
        {
            await botClient.SendMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "📭 У тебя пока нет покупок подарков.",
                cancellationToken: cancellationToken
            );
            return true;
        }
        
        var lastTransactions = transactions
            .OrderByDescending(t => t.TransactionDate)
            .Take(10)
            .ToList();

        var sb = new StringBuilder("🎁 *Твои последние покупки подарков*\n──────────────────────────────\n");

        foreach (var t in lastTransactions)
        {
            sb.AppendLine($"""
                           • 🎁 *Gift ID:* `{t.GiftId}`
                             💰 *Цена:* `{t.Price}`
                             📅 *Дата:* `{t.TransactionDate:yyyy-MM-dd HH:mm}`
                           """);
        }

        await botClient.SendMessageAsync(
            chatId: update.Message.Chat.Id,
            text: sb.ToString(),
            parseMode: "Markdown",
            cancellationToken: cancellationToken
        );

        return true;
    }
}
