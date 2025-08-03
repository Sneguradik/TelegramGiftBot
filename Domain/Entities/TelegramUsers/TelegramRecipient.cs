using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.TelegramUsers;

public class TelegramRecipient
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }
    public string? Username { get; set; } = string.Empty;
    public RecipientType RecipientType { get; set; }
    public long ClientId { get; set; }
    public TelegramClient Client { get; set; } = null!;
}

public enum RecipientType
{
    User,
    Channel
}