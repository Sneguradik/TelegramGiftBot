using System.Threading.Channels;
using Domain.Interfaces.Services.Telegram;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.BotAPI.GettingUpdates;

namespace Web.Controllers
{
    [Route("telegram")]
    [ApiController]
    [EnableCors("TelegramCors")]
    public class TelegramController(Channel<Update> channel) : ControllerBase
    {
        
        [HttpPost]
        public async Task<IActionResult> ReceiveUpdateAsync(Update update, CancellationToken cancellationToken = default)
        {
            await channel.Writer.WriteAsync(update, cancellationToken);
            return Ok();
        }
    }
}
