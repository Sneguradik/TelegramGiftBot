using System.Text.RegularExpressions;
using Domain.Entities.TelegramUsers;
using Domain.Interfaces.Repos;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Commands;

public class AddRecipientCommandHandler(
    ITelegramRecipientRepo recipientRepo,
    ITelegramClientRepo clientRepo,
    IUnitOfWork unitOfWork,
    ITelegramBotClient botClient
) : ICommandMessageHandler<Update>
{
    public const string CommandName = "addRecipient";

    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        var message = update.Message;
        if (message?.Text is null)
            return false;
        
        var match = Regex.Match(
            message.Text,
            @"\/addRecipient\s+id:(\d+)\s+type:(User|Channel)",
            RegexOptions.IgnoreCase
        );

        if (!match.Success)
        {
            await botClient.SendMessageAsync(
                chatId: message.Chat.Id,
                text: "Неверный формат команды. Используй:\n" +
                      "`/addRecipient id:<recipientId> type:<User|Channel>`",
                parseMode: "Markdown",
                cancellationToken: cancellationToken
            );
            return true;
        }

        var recipientId = long.Parse(match.Groups[1].Value);
        var type = match.Groups[2].Value.Equals("Channel", StringComparison.OrdinalIgnoreCase)
            ? RecipientType.Channel
            : RecipientType.User;

        var clientId = message.From!.Id;
        var client = await clientRepo.GetByIdAsync(clientId, false, cancellationToken);
        if (client is null)
        {
            await botClient.SendMessageAsync(
                chatId: message.Chat.Id,
                text: "Сначала зарегистрируйся через /start",
                cancellationToken: cancellationToken
            );
            return true;
        }

  
        var existing = await recipientRepo.FindAsync(
            r => r.ClientId == clientId && r.Id == recipientId && r.RecipientType == type,
            false,
            cancellationToken
        );

        if (existing.Any())
        {
            await botClient.SendMessageAsync(
                chatId: message.Chat.Id,
                text: $"Получатель с ID {recipientId} ({type}) уже существует.",
                cancellationToken: cancellationToken
            );
            return true;
        }
        
        var recipient = new TelegramRecipient
        {
            Id = recipientId,
            RecipientType = type,
            ClientId = clientId
        };

        await recipientRepo.AddAsync(recipient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await botClient.SendMessageAsync(
            chatId: message.Chat.Id,
            text: $"✅ Получатель с ID `{recipientId}` ({type}) успешно добавлен.",
            parseMode: "Markdown",
            cancellationToken: cancellationToken
        );

        return true;
    }
}