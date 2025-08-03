using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.TelegramUsers;

public class TelegramClientWallet
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public TelegramClient Client { get; set; } = null!;
    public long Amount { get; set; }
    [MaxLength(15)]
    public string Currency { get; set; } = null!;
}