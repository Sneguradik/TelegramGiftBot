using Domain.Interfaces.Services;

namespace Web.Workers;

public class AutoBuyWorker(IServiceProvider sp) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = sp.CreateScope();
        var autoBuyingService = scope.ServiceProvider.GetRequiredService<IAutoBuyingService>();
        await autoBuyingService.RunAsync(stoppingToken);
    }
}