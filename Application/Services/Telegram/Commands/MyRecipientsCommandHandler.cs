using System.Text.Json;
using Application.Services.Telegram.Callbacks;
using Domain.Entities;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Commands;

public class MyRecipientsCommandHandler(
    ITelegramRecipientRepo recipientRepo,
    ITelegramBotClient botClient)
    : ICommandMessageHandler<Update>
{
    public const string CommandName = "myRecipients";

    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        var userId = update.Message!.From!.Id;

        var recipients = await recipientRepo.GetByClientIdAsync(userId, false, cancellationToken);

        if (!recipients.Any())
        {
            await botClient.SendMessageAsync(
                chatId: userId, 
                text: "📭 У тебя пока нет сохранённых получателей.",
                cancellationToken: cancellationToken
            );
            return true;
        }

        foreach (var recipient in recipients)
        {

            var text = $"""
                        🎯 *Получатель #{recipient.Id}*
                        ──────────────────────
                        👤 *Тип:* {recipient.RecipientType}
                        🏷️ *Ник:* @{recipient.Username ?? "—"}
                        """;

            var inlineMarkup = new InlineKeyboardMarkup([
                [new InlineKeyboardButton("❌ Удалить получателя")
                {
                    CallbackData = JsonSerializer.Serialize(new CallbackData<long>
                    {
                        Name = DeleteRecipientCallBackHandler.CallbackName,
                        Data = recipient.Id
                    })
                }]
            ]);

            await botClient.SendMessageAsync(
                chatId: userId,
                text: text,
                parseMode: "Markdown",
                replyMarkup: inlineMarkup,
                cancellationToken: cancellationToken
            );
        }

        return true;
    }
}

