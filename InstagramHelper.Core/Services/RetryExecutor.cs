using Microsoft.Extensions.Logging;

namespace InstagramHelper.Core.Services
{
    public class RetryExecutor
    {
        private readonly ILogger<RetryExecutor> _logger;

        public RetryExecutor(ILogger<RetryExecutor> logger)
        {
            _logger = logger;
        }

        public async Task<T> Retry<T>(Func<Task<T>> operation, int retryTimes = 1, int delay = 1000)
        {
            for (int i = 0; i <= retryTimes; i++)
            {
                try
                {
                    return await operation();
                }
                catch (HttpRequestException)
                {
                    if (i == retryTimes)
                        throw;

                    _logger.LogWarning("Exception caught on attempt '{attempts}' - will retry after delay '{delay}'", i + 1, delay);

                    await Task.Delay(delay);
                }
            }

            throw new InvalidOperationException("Unexpected code execution path.");
        }
    }
}
