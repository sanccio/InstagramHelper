using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Quartz;
using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection.Extensions;
using InstagramHelper.Core.Services.TelegramServices.States;
using InstagramHelper.Core.Services.TelegramServices.UserService;
using InstagramHelper.Core.Services.TelegramServices;
using InstagramHelper.Core.Services.InstagramServices.Ig;
using InstagramHelper.Core.Services.TelegramServices.Handlers;
using InstagramHelper.Core.Services.TelegramServices.Actions;
using InstagramHelper.Core.Services.SchedulerService;
using InstagramHelper.Core.Services.SubscriptionsService;

namespace InstagramHelper.Core.Services
{
    public static class ServiceCollectionExtension
    {
        public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ITelegramUserService, TelegramUserService>();
            services.AddScoped<IIgService, IgService>();

            services.AddHttpClient<IIgApi, IgApi>(client =>
            {
                string baseAddress = configuration.GetValue<string>("IgApi:BaseAddress")!;

                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("User-Agent", "Chrome/120.0.0.0");
            });

            services.AddScoped<RetryExecutor>();
            services.AddScoped<StoriesScheduler>();

            services.AddHttpClient("InstaUserDataHandler");
            services.AddScoped<InstaUserDataHandler>();

            services.AddScoped<UpdateHandler>();

            services.AddScoped<EmptyState>();
            services.AddScoped<TimeInputState>();
            services.AddScoped<BotContext>();

            services.AddScoped<MessageHandler>();
            services.AddScoped<CallbackQueryHandler>();

            services.AddScoped<CallbackQueries>();
            services.AddScoped<Commands>();

            services.AddHttpClient("TelegramBotClient")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    string botToken = configuration.GetValue<string>("BotConfiguration:BotToken")!;

                    TelegramBotClientOptions options = new(botToken);
                    return new TelegramBotClient(botToken, httpClient);
                });

            services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
            });

            services.AddQuartzHostedService(q =>
            {
                q.WaitForJobsToComplete = true;
            });

            services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
        }
    }
}
