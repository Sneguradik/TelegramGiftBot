using Application.Services;
using Application.Services.Telegram;
using Application.Services.Telegram.Callbacks;
using Application.Services.Telegram.Commands;
using Application.Services.Telegram.Payments;
using Domain.Interfaces.Services;
using Domain.Interfaces.Services.Telegram;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Microsoft.Extensions.DependencyInjection;
using Telegram.BotAPI.GettingUpdates;
using IStarInvoiceService = Domain.Interfaces.Services.Telegram.IStarInvoiceService;

namespace Application.Di;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IGiftBuyingService, GiftBuyingService>();
        services.AddScoped<IStarInvoiceService, StarInvoiceService>();
        services.AddScoped<IAutoBuyingService, AutoBuyingService>();
        services.AddTelegram();
        return services;
    }

    private static IServiceCollection AddTelegram(this IServiceCollection services)
    {
        #region telegramCommands
        
        services.AddSingleton<IDefaultMessageHandler<Update>, DefaultMessageHandler>();
        
        services.AddKeyedScoped<ICommandMessageHandler<Update>, StartCommandHandler>(StartCommandHandler.CommandName);
        services.AddKeyedScoped<ICommandMessageHandler<Update>, HelpCommandHandler>(HelpCommandHandler.CommandName);
        services.AddKeyedScoped<ICommandMessageHandler<Update>, CreateGiftInvoiceCommandHandler>(CreateGiftInvoiceCommandHandler.CommandName);
        services.AddKeyedScoped<ICommandMessageHandler<Update>, BuyGiftCommandHandler>(BuyGiftCommandHandler.CommandName);
        services.AddKeyedScoped<ICommandMessageHandler<Update>, ListInvoicesCommandHandler>(ListInvoicesCommandHandler.CommandName);
        services.AddKeyedScoped<ICommandMessageHandler<Update>, MyGiftTransactionsCommandHandler>(MyGiftTransactionsCommandHandler.CommandName);
        services.AddKeyedScoped<ICommandMessageHandler<Update>, MyRecipientsCommandHandler>(MyRecipientsCommandHandler.CommandName);
        services.AddKeyedScoped<ICommandMessageHandler<Update>, TopUpBalanceHandler>(TopUpBalanceHandler.CommandName);
        services.AddKeyedScoped<ICommandMessageHandler<Update>, AddRecipientCommandHandler>(AddRecipientCommandHandler
            .CommandName);
        services.AddKeyedScoped<ICommandMessageHandler<Update>, MyBalanceCommandHandler>(MyBalanceCommandHandler.CommandName);

        services.AddKeyedScoped<ICallBackHandler<Update>, DeleteRecipientCallBackHandler>(DeleteRecipientCallBackHandler.CallbackName);
        services.AddKeyedScoped<ICallBackHandler<Update>, DeleteInvoiceCallbackHandler>(DeleteInvoiceCallbackHandler.CallbackName);
        services.AddKeyedScoped<IPaymentHandler<Update>, PaymentHandler>(PaymentHandler.Name);
        #endregion
        return services;
    }
}