using InstagramHelper.Core.Services.TelegramServices;
using InstagramHelper.Webhook.Filters;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace InstagramHelper.Webhook.Controllers
{
    public class BotController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<BotController> _logger;

        public BotController(ITelegramBotClient botClient, ILogger<BotController> logger)
        {
            _logger = logger;
            _botClient = botClient;
        }

        [HttpPost]
        [ValidateTelegramBot]
        public async Task<IActionResult> Post([FromBody] Update update,
                                              [FromServices] UpdateHandler handleUpdateService,
                                              CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update '{UpdateId}' is received", update.Id);
            await handleUpdateService.HandleUpdateAsync(_botClient, update, cancellationToken);
            return Ok();
        }
    }
}
