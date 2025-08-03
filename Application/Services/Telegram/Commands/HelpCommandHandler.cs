using Domain.Interfaces.Services.Telegram.TelegramUpdateHandlers;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace Application.Services.Telegram.Commands;

public class HelpCommandHandler(ITelegramBotClient client) : ICommandMessageHandler<Update>
{

    
    public const string CommandName = "help";
    
    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (update.Message is null) 
            return false;

        var commandReply = """
                           👋 *Привет!*  
                           Вот список команд для управления ботом:  

                           ───────────────
                           🚀 */start* – регистрация пользователя  
                           Создаёт кошелёк со звёздами и дефолтного получателя (твоя учётка).  

                           ℹ️ */help* – показать это сообщение  

                           💰 */myBalance* – посмотреть баланс всех кошельков  

                           ⭐ */topUpBalance* `amount:<stars>` – пополнить баланс в звёздах  

                           🎁 */buyGift* `giftId:<id>` `quantity:<qty>` `recipientId:<id>` – купить подарок  
                           🧾 */createGiftInvoice* `minPrice:<min>` `maxPrice:<max>` `quantity:<qty>` – создать инвойс на откуп подарков  

                           📜 */listInvoices* – мои активные инвойсы  
                           🎁 */myGiftTransactions* – история моих покупок  

                           👥 */myRecipients* – список получателей подарков  
                           ➕ */addRecipient* `id:<recipientId>` `type:<User|Channel>` – добавить получателя  
                           """;
        await client.SendMessageAsync(
            chatId: update.Message.Chat.Id,
            text: commandReply,
            parseMode: "Markdown",
            cancellationToken: cancellationToken
        );

        return true;
    }
}