using System.Threading.Channels;
using Application.Di;
using Domain.Interfaces.Services.Telegram;
using Infrastructure.Database;
using Infrastructure.DI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using Web.ApiServices.Authorization;
using Web.ApiServices.Config;
using Web.Workers;

namespace Web;

public static class ApplicationStartupExtension
{
    public static IServiceCollection AddConfiguredServices(this IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateUnbounded<Update>());
        services.AddInfrastructureServices();
        services.AddTelegramBot();
        services.AddHostedService<TelegramWorker>();
        services.AddHostedService<AutoBuyWorker>();
        services.AddApplicationServices();
        return services;
    }

    public static IServiceCollection AddConfiguredCors(this IServiceCollection services)
    {
        
        services.AddCors(options =>
        {
            options.AddPolicy("TelegramCors", cors => cors
                .AllowAnyOrigin()
                .WithMethods("POST")
                .WithHeaders("X-Telegram-Bot-Api-Secret-Token", "Content-Type")
                //.AllowCredentials()
            );
        });
            
        
        return services;
    }

    public static IServiceCollection AddConfiguredAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, TelegramSecretTokenHandler>();
        services.AddAuthorizationBuilder()
            .AddPolicy("TelegramPolicy", policy => policy
                .AddRequirements(new TelegramSecretTokenRequirement()));
        return services;
    }

    public static async Task PrepareAppAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var telegramBotClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        var giftBuyer = scope.ServiceProvider.GetRequiredService<ITelegramGiftBuyer>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<TelegramBotConfig>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync();
        
        await telegramBotClient.SetWebhookAsync(options.Value.WebhookUrl);

        await giftBuyer.InitAsync();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        app.UseCors("TelegramCors");
        
        app.UseHttpsRedirection();
        
        app.UseAuthorization();
        
        app.MapControllers();
    }
}