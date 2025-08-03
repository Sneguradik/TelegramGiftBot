using System.Linq.Expressions;
using Domain.Entities.TelegramUsers;
using Domain.Interfaces.Repos.TelegramUsers;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Telegram.BotAPI;

namespace Infrastructure.Telegram.Repos;

public class TelegramClientRepo(ApplicationDbContext context) : ITelegramClientRepo
{
    public async Task<bool> CheckByIdAsync(long id, CancellationToken cancellationToken = default) => await context
        .TelegramClients
        .AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<TelegramClient?> GetByIdAsync(long id, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    

    public async Task<IEnumerable<TelegramClient>> FindAsync(Expression<Func<TelegramClient, bool>> predicate, bool withTracking = false, CancellationToken cancellationToken = default) =>
        await Query(withTracking).Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(TelegramClient client, CancellationToken cancellationToken = default) =>
        await context.TelegramClients.AddAsync(client, cancellationToken);

    public void Update(TelegramClient client) =>
        context.TelegramClients.Update(client);

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default) =>
        await context.TelegramClients.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);

    private IQueryable<TelegramClient> Query(bool withTracking) =>
        withTracking ? context.TelegramClients : context.TelegramClients.AsNoTracking();
}
