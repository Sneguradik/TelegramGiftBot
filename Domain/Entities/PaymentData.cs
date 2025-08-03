namespace Domain.Entities;

public class PaymentData
{
    public string Name { get; set; } = string.Empty;
    public long WalletId { get; set; }
    public double Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 