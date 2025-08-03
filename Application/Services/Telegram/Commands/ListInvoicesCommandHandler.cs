using System.Text.Json;
using Application.Services.Telegram.Callbacks;
using Domain.Entities;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Commands;

public class ListInvoicesCommandHandler(IGiftInvoiceRepo invoiceRepo, ITelegramBotClient botClient) 
    : ICommandMessageHandler<Update>
{
    public const string CommandName = "listInvoices";

    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        var userId = update.Message!.From!.Id;
        var invoices = await invoiceRepo.GetByUserAsync(userId, false, cancellationToken);

        if (!invoices.Any())
        {
            await botClient.SendMessageAsync(
                chatId: userId,
                text: "📭 У тебя пока нет активных инвойсов.",
                cancellationToken: cancellationToken
            );
            return true;
        }

        foreach (var invoice in invoices)
        {
            var text = $"""
                        🧾 *Инвойс #{invoice.Id}*
                        ──────────────────────
                        💰 *Диапазон цен:* `{invoice.MinPrice}-{invoice.MaxPrice}`
                        📦 *Количество:* `{invoice.Amount}`
                        📅 *Дата:* `{invoice.Created:yyyy-MM-dd HH:mm}`
                        """;

            var keyboard = new InlineKeyboardMarkup([
                [new InlineKeyboardButton("❌ Удалить инвойс")
                {
                    CallbackData = JsonSerializer.Serialize(
                        new CallbackData<long>
                        {
                            Name = DeleteInvoiceCallbackHandler.CallbackName,
                            Data = invoice.Id
                        })
                }]
            ]);

            await botClient.SendMessageAsync(
                chatId: userId,
                text: text,
                parseMode: "Markdown",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );
        }

        return true;
    }
}
