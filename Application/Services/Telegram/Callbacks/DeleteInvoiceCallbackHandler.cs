using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces.Repos;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;

namespace Application.Services.Telegram.Callbacks;

public class DeleteInvoiceCallbackHandler(
    IGiftInvoiceRepo invoiceRepo,
    IUnitOfWork unitOfWork,
    ITelegramBotClient botClient
) : ICallBackHandler<Update>
{
    public const string CallbackName = "deleteInvoice";

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
                callback.Id,
                "⚠️ Ошибка при разборе данных кнопки.",
                cancellationToken: cancellationToken
            );
            return false;
        }

        if (content is null)
            return false;

        var invoiceId = content.Data;
        var invoice = await invoiceRepo.GetByIdAsync(invoiceId, cancellationToken: cancellationToken);
        if (invoice is null)
        {
            await botClient.AnswerCallbackQueryAsync(
                callback.Id,
                "❌ Инвойс уже удалён или не существует.",
                cancellationToken: cancellationToken
            );
            return true;
        }
        
        var userId = callback.From.Id;
        if (invoice.RecipientId != userId)
        {
            await botClient.AnswerCallbackQueryAsync(
                callback.Id,
                "🚫 Ты не можешь удалять чужие инвойсы.",
                cancellationToken: cancellationToken
            );
            return true;
        }

        await invoiceRepo.DeleteAsync(invoiceId, cancellationToken);
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
            "✅ Инвойс удалён",
            cancellationToken: cancellationToken
        );

        return true;
    }
}
