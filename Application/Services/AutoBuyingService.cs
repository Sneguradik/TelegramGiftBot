using System.Collections.Concurrent;
using Domain.Entities.TelegramUsers;
using Domain.Entities.TelegramValueTransfer;
using Domain.Interfaces.Repos;
using Domain.Interfaces.Repos.TelegramValueTransfer;
using Domain.Interfaces.Services;
using Domain.Interfaces.Services.Telegram;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AutoBuyingService(
    IGiftBuyingService giftBuyingService,
    ITelegramGiftBuyer telegramGiftBuyer,
    IGiftTransactionRepo giftTransactionRepo,
    IGiftInvoiceRepo giftInvoiceRepo,
    IUnitOfWork uof,
    ILogger<AutoBuyingService> logger
) : IAutoBuyingService
{
    private HashSet<long> _knownGifts = new();
    private readonly ConcurrentQueue<GiftTransaction> _transactionQueue = new();

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("🚀 AutoBuyingService запущен");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var giftList = (await telegramGiftBuyer.GetAvailableGiftsAsync(cancellationToken)).ToArray();
                
                var newGiftIds = giftList
                    .Select(x => x.Id)
                    .ToHashSet()
                    .Except(_knownGifts)
                    .ToArray();

                if (!newGiftIds.Any())
                {
                    await FlushTransactionsAsync(cancellationToken);
                    await Task.Delay(300, cancellationToken); 
                    continue;
                }

                
                var giftsToBuy = giftList
                    .Where(x => newGiftIds.Contains(x.Id))
                    .Where(x => x is {  CurrentSupply: > 0 ,Limited: true })
                    .ToArray();

                if (giftsToBuy.Length == 0)
                {
                    await Task.Delay(100, cancellationToken);
                    continue;
                }

                foreach (var giftToBuy in giftsToBuy)
                {
                    Console.WriteLine($"Id: {giftToBuy.Id} , Price: {giftToBuy.Price}, Supply: {giftToBuy.CurrentSupply}/{giftToBuy.TotalSupply}");
                }
                
                var invoices = (await giftInvoiceRepo.GetAllWithRecipientsAndClientsAsync(cancellationToken))
                    .OrderByDescending(x => x.Recipient.Client.Priority)
                    .ToArray();

                logger.LogInformation("📄 Загружено {Count} инвойсов для обработки", invoices.Length);
                
                var grouped = invoices.GroupBy(x => x.Recipient.Client.Priority)
                    .OrderByDescending(g => g.Key);

                foreach (var priorityGroup in grouped)
                {
                    var tasks = new List<Task>();
                    foreach (var invoice in priorityGroup)
                    {
                        tasks.Add(Task.Run(() => ProcessInvoiceAsync(invoice, giftsToBuy, cancellationToken), cancellationToken));
                    }

                    await Task.WhenAll(tasks);
                    
                    await FlushTransactionsAsync(cancellationToken);
                }

                _knownGifts = giftList.Select(x => x.Id).ToHashSet();
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("⏹ AutoBuyingService остановлен по CancellationToken");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Ошибка в AutoBuyingService");
                await Task.Delay(1000, cancellationToken);
            }
        }

        await FlushTransactionsAsync(cancellationToken);
        logger.LogInformation("✅ AutoBuyingService завершил работу");
    }

    private async Task ProcessInvoiceAsync(GiftInvoice invoice, Gift[] gifts, CancellationToken cancellationToken)
    {
        var suitableGifts = gifts
            .Where(x => invoice.MinPrice <= x.Price && x.Price <= invoice.MaxPrice)
            .OrderByDescending(x => x.Price)
            .ToArray();

        if (!suitableGifts.Any())
        {
            logger.LogDebug("Инвойс #{InvoiceId}: нет подходящих подарков", invoice.Id);
            return;
        }

        logger.LogInformation("📄 Обрабатываю инвойс #{InvoiceId}, осталось купить {Amount} шт.",
            invoice.Id, invoice.Amount);

        foreach (var gift in suitableGifts)
        {
            var buyTasks = Enumerable
                .Range(0, invoice.Amount)
                .Select(_ => giftBuyingService.BuyGiftForRecipientAsync(gift, invoice.Recipient, cancellationToken))
                .ToArray();

            var results = await Task.WhenAll(buyTasks);
            var successful = results.Where(x => x is not null).Cast<GiftTransaction>().ToList();

            invoice.Amount -= successful.Count;
            foreach (var tx in successful) _transactionQueue.Enqueue(tx);

            logger.LogInformation("🎁 Куплено {Count} подарков {GiftId} для инвойса #{InvoiceId}",
                successful.Count, gift.Id, invoice.Id);

            if (invoice.Amount <= 0)
            {
                logger.LogInformation("✅ Инвойс #{InvoiceId} полностью исполнен", invoice.Id);
                break;
            }
        }
    }

    private async Task FlushTransactionsAsync(CancellationToken cancellationToken)
    {
        if (_transactionQueue.IsEmpty) return;

        var txs = new List<GiftTransaction>();
        while (_transactionQueue.TryDequeue(out var tx)) txs.Add(tx);

        if (txs.Count == 0) return;

        foreach (var tx in txs)
            await giftTransactionRepo.AddAsync(tx, cancellationToken);

        await uof.SaveChangesAsync(cancellationToken);
        logger.LogInformation("💾 Сохранено {Count} транзакций", txs.Count);
    }
}
