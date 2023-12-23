using InstagramHelper.Core.Services.TelegramServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace InstagramHelper.Polling
{
    public class PollingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PollingService> _logger;

        public PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>(),
                ThrowPendingUpdates = true
            };

            var botClient = _serviceProvider.GetRequiredService<ITelegramBotClient>();
            var handler = _serviceProvider.GetRequiredService<UpdateHandler>();

            var me = await botClient.GetMeAsync(cancellationToken);
            _logger.LogInformation("Start listening for @{BotName}", me.Username);

            await botClient.ReceiveAsync(
                updateHandler: handler.HandleUpdateAsync,
                pollingErrorHandler: handler.HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cancellationToken
            );
        }
    }
}
