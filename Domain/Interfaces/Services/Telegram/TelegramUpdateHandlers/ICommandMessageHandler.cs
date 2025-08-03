namespace Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;

public interface ICommandMessageHandler<in TUpdate> : ITelegramUpdateHandler<TUpdate>
{
    
}