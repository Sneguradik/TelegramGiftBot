using System.Linq.Expressions;
using Domain.Entities.TelegramUsers;
using Domain.Interfaces.Repos.TelegramUsers;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Telegram.Repos;

public class TelegramRecipientRepo(ApplicationDbContext context) : ITelegramRecipientRepo
{
    public async Task<TelegramRecipient?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IEnumerable<TelegramRecipient>> GetByClientIdAsync(long clientId, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .AsSplitQuery()
            .Where(r => r.ClientId == clientId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<TelegramRecipient>> GetByTypeAsync(RecipientType type, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .Where(r => r.RecipientType == type)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<TelegramRecipient>> FindAsync(Expression<Func<TelegramRecipient, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking)
            .Where(predicate)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

    public async Task AddAsync(TelegramRecipient recipient, CancellationToken cancellationToken = default) =>
        await context.TelegramRecipients
            .AddAsync(recipient, cancellationToken);

    public void Update(TelegramRecipient recipient) =>
        context.TelegramRecipients.Update(recipient);

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default) =>
        await context.TelegramRecipients.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);

    private IQueryable<TelegramRecipient> Query(bool withTracking) =>
        withTracking ? context.TelegramRecipients : context.TelegramRecipients.AsNoTracking();
}
