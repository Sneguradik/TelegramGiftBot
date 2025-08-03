using System.Linq.Expressions;
using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Telegram.Repos;

public class GiftInvoiceRepo(ApplicationDbContext context) : IGiftInvoiceRepo
{
    public async Task<GiftInvoice?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IEnumerable<GiftInvoice>> GetAllWithRecipientsAndClientsAsync(
        CancellationToken cancellationToken = default) => await context
        .GiftInvoices
        .Include(x => x.Recipient)
        .ThenInclude(x => x.Client)
        .AsSplitQuery()
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<GiftInvoice>> GetByRecipientIdAsync(long recipientId, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .Where(i => i.RecipientId == recipientId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    
    public async Task<IEnumerable<GiftInvoice>> GetByUserAsync(long userId, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .Where(i => i.Recipient.Client.Id == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<GiftInvoice>> FindAsync(Expression<Func<GiftInvoice, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).Where(predicate).ToListAsync(cancellationToken);

    public Task AddAsync(GiftInvoice invoice, CancellationToken cancellationToken = default)
    { 
        context.GiftInvoices.Attach(invoice);
        return Task.CompletedTask;
    }
        

    public void Update(GiftInvoice invoice) =>
        context.GiftInvoices.Update(invoice);

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default) =>
        await context.GiftInvoices.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);

    private IQueryable<GiftInvoice> Query(bool withTracking) =>
        withTracking ? context.GiftInvoices : context.GiftInvoices.AsNoTracking();
}
