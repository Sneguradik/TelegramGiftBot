using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces.Services.Telegram;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Microsoft.Extensions.DependencyInjection;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.Payments;

namespace Infrastructure.Telegram.Services;

public class TelegramUpdateRouter(IDefaultMessageHandler<Update> defaultMessageHandler, IServiceProvider serviceProvider, ITelegramBotClient botClient) : ITelegramUpdateRouter<Update>, IDisposable
{
    private readonly IServiceScope _scope = serviceProvider.CreateScope();
    
    public async Task RouteUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (update.Message?.Text != null)
        {
            if (update.Message.Text.StartsWith('/'))
            {
                
                var commandHandler = _scope.ServiceProvider
                    .GetKeyedService<ICommandMessageHandler<Update>>(update.Message.Text
                        .Split(" ")
                        .First()[1..]);

                if (commandHandler is not null) await commandHandler.HandleUpdateAsync(update, cancellationToken);
            }
            
            else await defaultMessageHandler.HandleUpdateAsync(update, cancellationToken);
        }
        else if (update.PreCheckoutQuery is not null)
        {
            await botClient
                .AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id, true, cancellationToken: cancellationToken);
        }
        
        else if (update.Message?.SuccessfulPayment is not null)
        {
            
            var paymentData = JsonSerializer.Deserialize<PaymentData>(update.Message.SuccessfulPayment.InvoicePayload);
            if (paymentData is null) return;
            var commandHandler = _scope.ServiceProvider.GetKeyedService<IPaymentHandler<Update>>(paymentData.Name);
            if (commandHandler is not null) await commandHandler.HandleUpdateAsync(update, cancellationToken);
        }
        else if (update.CallbackQuery is not null)
        {
            if (update.CallbackQuery.Data is null) return;
            var callbackData = JsonSerializer.Deserialize<CallbackData<JsonElement>>(update.CallbackQuery.Data);
            if (callbackData is null) return;
            var callbackHandler = _scope.ServiceProvider.GetKeyedService<ICallBackHandler<Update>>(callbackData.Name);
            if(callbackHandler is not null) await callbackHandler.HandleUpdateAsync(update, cancellationToken);
        }
    }

    public void Dispose()
    {
        _scope.Dispose();
    }
}
