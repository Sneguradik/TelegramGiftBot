using System.Text;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Commands;

public class MyBalanceCommandHandler(
    ITelegramClientWalletRepo walletRepo,
    ITelegramBotClient botClient
) : ICommandMessageHandler<Update>
{
    public const string CommandName = "myBalance";

    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (update.Message?.From is null)
            return false;

        var userId = update.Message.From.Id;

        var wallet = await walletRepo.GetByClientIdAsync(userId, false, cancellationToken);

        if (wallet is null)
        {
            await botClient.SendMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "😕 У тебя ещё нет кошелька. Зарегистрируйся через /start",
                cancellationToken: cancellationToken
            );
            return true;
        }

        await botClient.SendMessageAsync(
            chatId: update.Message.Chat.Id,
            text: $"💰 Твой баланс: *{wallet.Amount}* {wallet.Currency}",
            parseMode: "Markdown",
            cancellationToken: cancellationToken
        );

        return true;
    }
}

