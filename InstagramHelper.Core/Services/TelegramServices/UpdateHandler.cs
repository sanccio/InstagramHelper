using InstagramHelper.Core.Enums;
using InstagramHelper.Core.Models;
using InstagramHelper.Core.Services.TelegramServices.States;
using InstagramHelper.Core.Services.TelegramServices.UserService;
using InstagramHelper.Core.Services.TelegramServices.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace InstagramHelper.Core.Services.TelegramServices
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ITelegramUserService _tgUserService;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly TimeInputState _timeInputState;
        private readonly EmptyState _emptyState;
        private readonly BotContext _botContext;

        public UpdateHandler(
            ITelegramUserService tgUserService,
            ILogger<UpdateHandler> logger,
            TimeInputState timeInputState,
            EmptyState emptyState,
            BotContext botContext)
        {
            _tgUserService = tgUserService;
            _logger = logger;
            _timeInputState = timeInputState;
            _emptyState = emptyState;
            _botContext = botContext;
        }


        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Chat chat = update.GetChat();            
            await _tgUserService.SaveUserIfNotExistsAsync(chat);

            TelegramUser user = await _tgUserService.GetUserByIdAsync(update.GetChatId());
            _botContext.TelegramUser = user;

            try
            {
                var action = user.State switch
                {
                    State.Empty => _emptyState.HandleState(_botContext, update, cancellationToken),
                    State.WaitingForTimeInput => _timeInputState.HandleState(_botContext, update, cancellationToken),
                    _ => Task.CompletedTask
                };
                await action;
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal Server Error: {Ex}.", ex);

                await botClient.SendTextMessageAsync(
                    chatId: update.GetChatId(),
                    text: BotResponse.InternalServerError,
                    cancellationToken: cancellationToken);
            }
        }


        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogCritical("HandleError: {ErrorMessage}", ErrorMessage);

            return Task.CompletedTask;
        }
    }
}
