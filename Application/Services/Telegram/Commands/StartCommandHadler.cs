using Domain.Constants;
using Domain.Entities.TelegramUsers;
using Domain.Interfaces.Repos;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Commands;

public class StartCommandHandler(ITelegramBotClient botClient, ITelegramClientRepo telegramClientRepo, ITelegramClientWalletRepo walletRepo, ITelegramRecipientRepo recipientRepo, IUnitOfWork unitOfWork, ILogger<StartCommandHandler> logger) : ICommandMessageHandler<Update>
{
    
    public const string CommandName = "start";

    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (update.Message?.From is null) return false;

        if (!await telegramClientRepo.CheckByIdAsync(update.Message!.From!.Id, cancellationToken))
        {
            var client = new TelegramClient
            {
                Id = update.Message.From.Id,
                FirstName = update.Message.From.FirstName,
                Username = update.Message.From.Username,
            };
    
            var recipient = new TelegramRecipient
            {
                Id = update.Message.From.Id,
                Username = update.Message.From.Username,
                RecipientType = RecipientType.User,
                Client = client
            };
    
            var wallet = new TelegramClientWallet
            {
                Client = client,
                Amount = 0,
                Currency = CurrencyConstants.Stars
            };
    
            await telegramClientRepo.AddAsync(client, cancellationToken);
            await walletRepo.AddAsync(wallet, cancellationToken);
            await recipientRepo.AddAsync(recipient, cancellationToken);
            
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            logger.LogInformation($"Successfully added new telegram client {client.Id} - {client.Username}");
        }
        
        await botClient
            .SendMessageAsync(update.Message.Chat.Id, "Вы успешно зарегистрированы. Нажмите /help для списка команд.", cancellationToken: cancellationToken);
        
        return true;
    }
}