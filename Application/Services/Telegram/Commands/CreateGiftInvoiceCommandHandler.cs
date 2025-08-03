using System.Globalization;
using System.Text.RegularExpressions;
using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Repos;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Commands;


public class CreateGiftInvoiceCommandHandler(
    ITelegramBotClient botClient,
    ITelegramRecipientRepo recipientRepo,
    IGiftInvoiceRepo invoiceRepo,
    IUnitOfWork unitOfWork
) : ICommandMessageHandler<Update>
{
    public const string CommandName = "createGiftInvoice";

    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        var text = update.Message?.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            await botClient.SendMessageAsync(update.Message!.Chat.Id,
                "Команда должна содержать параметры. Пример:\n" +
                "/createGiftInvoice minPrice:10 maxPrice:50 quantity:3",
                cancellationToken: cancellationToken);
            return true;
        }

        var match = Regex.Match(text,
            @"\/createGiftInvoice\s+minPrice:(\d+(?:\.\d+)?)\s+maxPrice:(\d+(?:\.\d+)?)\s+quantity:(\d+)",
            RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            await botClient.SendMessageAsync(update.Message!.Chat.Id,
                "Неверный формат команды. Пример:\n" +
                "/createGiftInvoice minPrice:10 maxPrice:50 quantity:3",
                cancellationToken: cancellationToken);
            return true;
        }

        if (!double.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var minPrice) ||
            !double.TryParse(match.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var maxPrice) ||
            !int.TryParse(match.Groups[3].Value, out var quantity))
        {
            await botClient.SendMessageAsync(update.Message!.Chat.Id,
                "Не удалось разобрать числовые значения. Убедитесь, что minPrice, maxPrice и quantity корректны.",
                cancellationToken: cancellationToken);
            return true;
        }

        if (minPrice <= 0 || maxPrice <= 0)
        {
            await botClient.SendMessageAsync(update.Message!.Chat.Id,
                "Минимальная и максимальная цена должны быть положительными.",
                cancellationToken: cancellationToken);
            return true;
        }

        if (minPrice > maxPrice)
        {
            await botClient.SendMessageAsync(update.Message!.Chat.Id,
                "Минимальная цена не может быть больше максимальной.",
                cancellationToken: cancellationToken);
            return true;
        }

        if (quantity <= 0 || quantity > 100)
        {
            await botClient.SendMessageAsync(update.Message!.Chat.Id,
                "Количество должно быть положительным и не больше 100.",
                cancellationToken: cancellationToken);
            return true;
        }

        var clientId = update.Message!.From!.Id;
        var recipient = await recipientRepo.GetByIdAsync(clientId, cancellationToken: cancellationToken);
        if (recipient is null)
        {
            await botClient.SendMessageAsync(update.Message.Chat.Id,
                "Сначала зарегистрируйся через /start, чтобы создать получателя.",
                cancellationToken: cancellationToken);
            return true;
        }

        var invoice = new GiftInvoice
        {
            RecipientId = recipient.Id,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Amount = quantity,
            Created = DateTime.UtcNow
        };

        await invoiceRepo.AddAsync(invoice, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await botClient.SendMessageAsync(update.Message.Chat.Id,
            $"Инвойс успешно создан!\n" +
            $"Диапазон цен: {minPrice}-{maxPrice}\n" +
            $"Количество: {quantity}",
            cancellationToken: cancellationToken);

        return true;
    }
}