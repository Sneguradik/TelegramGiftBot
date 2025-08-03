using Domain.Entities.TelegramUsers;
using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> context) : DbContext(context)
{
    public DbSet<TelegramClient> TelegramClients { get; set; }
    public DbSet<TelegramRecipient>  TelegramRecipients { get; set; }
    public DbSet<TelegramClientWallet> TelegramClientWallets { get; set; }
    public DbSet<GiftInvoice> GiftInvoices { get; set; }
    public DbSet<GiftTransaction> GiftTransactions { get; set; }
    public DbSet<PaymentInvoice> PaymentInvoices { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TelegramClientWallet>()
            .HasOne(w => w.Client)
            .WithOne()
            .HasForeignKey<TelegramClientWallet>(w => w.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TelegramRecipient>()
            .HasOne(r => r.Client)
            .WithMany()
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GiftInvoice>()
            .HasOne(i => i.Recipient)
            .WithMany()
            .HasForeignKey(i => i.RecipientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GiftTransaction>()
            .HasOne(t => t.Recipient)
            .WithMany()
            .HasForeignKey(t => t.RecipientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PaymentInvoice>()
            .HasOne(i => i.Wallet)
            .WithMany()
            .HasForeignKey(i => i.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(t => t.Wallet)
            .WithMany()
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}