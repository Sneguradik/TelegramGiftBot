using System.Text.RegularExpressions;
using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Services.Telegram;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Commands;

public class BuyGiftCommandHandler(ITelegramBotClient botClient, ITelegramRecipientRepo recipientRepo, IGiftBuyingService buyingService, ITelegramGiftBuyer buyer) : ICommandMessageHandler<Update>
{
    public const string CommandName = "buyGift";

    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (update.Message?.Text is not { } text) return false;

        var match = Regex.Match(text, @"\/buyGift\s+giftId:(\d+)\s+quantity:(\d+)\s+recipientId:(\d+)", RegexOptions.IgnoreCase);
        if (!match.Success) return false;

        var giftId = long.Parse(match.Groups[1].Value);
        var quantity = int.Parse(match.Groups[2].Value);
        var recipientId = long.Parse(match.Groups[3].Value);

        var gifts = await buyer.GetAvailableGiftsAsync(cancellationToken);
        var recipient = await recipientRepo.GetByIdAsync(recipientId, false, cancellationToken);
        
        var gift = gifts.FirstOrDefault(g => g.Id == giftId);
        
        if (gift == null || recipient is null) return false;
        
        var transaction = await buyingService.BuyGiftForRecipientAsync(gift, recipient, cancellationToken);
        if (transaction is null) return false;

        await botClient
            .SendMessageAsync(update.Message.Chat.Id,$"Подарков: {quantity} успешно отправлены.", cancellationToken: cancellationToken);
        return true;
    }
}