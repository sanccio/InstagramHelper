using InstagramHelper.Core.Services;
using InstagramHelper.Webhook;
using InstagramHelper.Webhook.Controllers;

var builder = WebApplication.CreateBuilder(args);

var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
builder.Services.Configure<BotConfiguration>(botConfigurationSection);

var botConfiguration = botConfigurationSection.Get<BotConfiguration>()!;

builder.Services.AddLogging(builder => 
    builder
        .AddSimpleConsole(c => c.TimestampFormat = "[dd/MM/yy HH:mm:ss] ")
        .SetMinimumLevel(LogLevel.Debug));

IConfiguration config = builder.Configuration;
builder.Services.AddCoreServices(config);

builder.Services.AddHostedService<ConfigureWebhook>();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.UseSwagger();
//app.UseSwaggerUI();

app.MapBotWebhookRoute<BotController>(route: botConfiguration.Route);
app.MapControllers();
app.Run();

#pragma warning disable CA1050
public class BotConfiguration
#pragma warning restore CA1050
{
    public static readonly string Configuration = "BotConfiguration";

    public string BotToken { get; init; } = default!;
    public string HostAddress { get; init; } = default!;
    public string Route { get; init; } = default!;
    public string SecretToken { get; init; } = default!;
}