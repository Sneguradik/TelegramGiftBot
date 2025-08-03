namespace Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;

public interface ITelegramUpdateHandler<in TUpdate>
{
    Task<bool> HandleUpdateAsync(TUpdate update, CancellationToken cancellationToken = default);
}