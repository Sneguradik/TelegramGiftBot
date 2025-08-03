using Domain.Entities.TelegramUsers;

namespace Domain.Interfaces.Services;

public interface IAutoBuyingService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}