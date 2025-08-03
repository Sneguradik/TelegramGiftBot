using Domain.Entities.TelegramUsers;

namespace Domain.Interfaces.Services.Telegram;

public interface ITelegramUpdateRouter<in TUpdate>
{
    Task RouteUpdateAsync(TUpdate update, CancellationToken cancellationToken = default);
}