using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram;

public class DefaultMessageHandler(ITelegramBotClient botClient, ILogger<DefaultMessageHandler> logger) 
    : IDefaultMessageHandler<Update>
{
    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        var message = update.Message;
        if (message is null) 
            return false;

        string senderInfo;
        
        // Если сообщение пришло от имени канала или группы
        if (message.SenderChat is not null)
        {
            senderInfo = $"📢 Сообщение от канала/чата: <b>{message.SenderChat.Title}</b>\n" +
                         $"ID: <code>{message.SenderChat.Id}</code>";
        }
        else if (message.From is not null)
        {
            senderInfo = $"👤 Отправитель: <b>@{message.From.Username ?? "без ника"}</b>\n" +
                         $"ID пользователя: <code>{message.From.Id}</code>";
        }
        else
        {
            senderInfo = "❓ Отправитель неизвестен";
        }

        var reply = $"""
                     ⚡ <b>Сообщение получено!</b>

                     {senderInfo}

                     ℹ️ Чтобы узнать список доступных команд, используй <b>/help</b>.
                     """;

        await botClient.SendMessageAsync(
            chatId: message.Chat.Id,
            text: reply,
            parseMode: "HTML",
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "Default message from chat {ChatId} processed. Sender info: {SenderInfo}", 
            message.Chat.Id, senderInfo);

        return true;
    }
}