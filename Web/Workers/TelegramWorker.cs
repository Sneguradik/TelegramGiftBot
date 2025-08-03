using System.Threading.Channels;
using Domain.Interfaces.Services.Telegram;
using Telegram.BotAPI.GettingUpdates;

namespace Web.Workers;

public class TelegramWorker(Channel<Update> channel, ITelegramUpdateRouter<Update> router) : BackgroundService
{
    private readonly SemaphoreSlim _semaphore = new (8);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var update in channel.Reader.ReadAllAsync(stoppingToken))
        {
            await _semaphore.WaitAsync(stoppingToken);

            _ = Task.Run(async () =>
            {
                try
                {
                    await router.RouteUpdateAsync(update, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    _semaphore.Release();
                }
            }, stoppingToken);
        }
    }
}