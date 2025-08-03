using Domain.Interfaces.Repos;
using Domain.Interfaces.Repos.TelegramUsers;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Domain.Interfaces.Services.Telegram;
using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Infrastructure.Database;
using Infrastructure.Telegram.Repos;
using Infrastructure.Telegram.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace Infrastructure.DI;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IGiftInvoiceRepo, GiftInvoiceRepo>();
        services.AddScoped<IGiftTransactionRepo, GiftTransactionRepo>();
        services.AddScoped<IPaymentInvoiceRepo, PaymentInvoiceRepo>();
        services.AddScoped<IPaymentTransactionRepo, PaymentTransactionRepo>();
        services.AddScoped<ITelegramClientRepo, TelegramClientRepo>();
        services.AddScoped<ITelegramClientWalletRepo, TelegramClientWalletRepo>();
        services.AddScoped<ITelegramRecipientRepo, TelegramRecipientRepo>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        return services;
    }

    public static IServiceCollection AddTelegramBot(this IServiceCollection services)
    {
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>( _ => 
                    new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_API_KEY") ?? 
                                          throw new ArgumentNullException("TELEGRAM_API_KEY")));
        services.AddSingleton<ITelegramGiftBuyer, TelegramGiftBuyer>();
        services.AddSingleton<ITelegramUpdateRouter<Update>, TelegramUpdateRouter>();
        return services;
    }

    public static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt=>
            opt.UseNpgsql(configuration.GetConnectionString("MainDb")));
        return services;
    }
}