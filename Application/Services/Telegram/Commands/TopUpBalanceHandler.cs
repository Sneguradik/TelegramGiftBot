using System.Globalization;
using System.Text.RegularExpressions;
using Domain.Entities.TelegramUsers;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Services.Telegram;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Commands;

public class TopUpBalanceHandler(ITelegramBotClient botClient, IStarInvoiceService starInvoiceService, ITelegramClientRepo clientRepo) : ICommandMessageHandler<Update>
{
    public const string CommandName = "topUpBalance";
    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        var message = update.Message;
        
        Console.WriteLine(message.Text);
        if (message?.Text is null || message.From is null)
            return false;

        if (!message.Text.StartsWith($"/{CommandName}"))
            return false;
        
        var match = Regex.Match(message.Text, @"\/topUpBalance\s+amount:(\d+)", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            await botClient.SendMessageAsync(
                chatId: message.Chat.Id,
                text: "⚠️ Укажите сумму пополнения. Пример: `/topUpBalance amount:500`",
                cancellationToken: cancellationToken);
            return false;
        }
        
        var amountInStars = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        
        var client = await clientRepo.GetByIdAsync(message.Chat.Id, cancellationToken: cancellationToken);
        
        if (client is null) return false;
        
        await starInvoiceService
            .CreateInvoice(amountInStars, client, cancellationToken);
        
        return true;
    }
}