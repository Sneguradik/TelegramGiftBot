namespace Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;

public interface ICallBackHandler<in TUpdate> : ICommandMessageHandler<TUpdate>
{
    
}