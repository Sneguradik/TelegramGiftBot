using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces.Repos;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;

namespace Application.Services.Telegram.Callbacks;

public class DeleteRecipientCallBackHandler(
    ITelegramRecipientRepo recipientRepo,
    IUnitOfWork unitOfWork,
    ITelegramBotClient botClient
) : ICallBackHandler<Update>
{
    public const string CallbackName = "deleteRecipient";

    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        var callback = update.CallbackQuery;
        if (callback?.Data is null)
            return false;

        CallbackData<long>? content;
        try
        {
            content = JsonSerializer.Deserialize<CallbackData<long>>(callback.Data);
        }
        catch
        {
            await botClient.AnswerCallbackQueryAsync(
                callback!.Id,
                "⚠️ Ошибка при разборе данных кнопки.",
                cancellationToken: cancellationToken
            );
            return false;
        }

        if (content is null)
            return false;

        var recipientId = content.Data;
        var recipient = await recipientRepo.GetByIdAsync(recipientId, cancellationToken: cancellationToken);
        if (recipient is null)
        {
            await botClient.AnswerCallbackQueryAsync(
                callback.Id,
                "❌ Получатель уже удалён или не существует.",
                cancellationToken: cancellationToken
            );
            return true;
        }

        var userId = callback.From.Id;

        if (recipient.ClientId != userId)
        {
            await botClient.AnswerCallbackQueryAsync(
                callback.Id,
                "🚫 Ты не можешь удалять чужих получателей.",
                cancellationToken: cancellationToken
            );
            return true;
        }
        
        if (recipient.Id == userId)
        {
            await botClient.AnswerCallbackQueryAsync(
                callback.Id,
                "⚠️ Ты не можешь удалить себя как получателя.",
                cancellationToken: cancellationToken
            );
            return true;
        }

        await recipientRepo.DeleteAsync(recipientId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        if (callback.Message != null)
        {
            await botClient.DeleteMessageAsync(
                chatId: callback.Message.Chat.Id,
                messageId: callback.Message.MessageId,
                cancellationToken: cancellationToken
            );
        }

        await botClient.AnswerCallbackQueryAsync(
            callback.Id,
            "✅ Получатель удалён",
            cancellationToken: cancellationToken
        );

        return true;
    }
}
