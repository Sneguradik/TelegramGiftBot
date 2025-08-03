using Domain.Entities.TelegramUsers;

namespace Domain.Interfaces.Services.Telegram;

public interface IStarInvoiceService
{
    Task CreateInvoice(int amount, TelegramClient client, CancellationToken ct = default);
}