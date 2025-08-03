using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.TelegramUsers;

public class TelegramClient
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }
    public string? Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public float Priority { get; set; }
}