using InstagramHelper.Core;
using InstagramHelper.Core.Services;
using InstagramHelper.Polling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
{
    services.AddLogging(builder => 
        builder
            .AddSimpleConsole(c => c.TimestampFormat = "[dd/MM/yy HH:mm:ss] ")
            .SetMinimumLevel(LogLevel.Debug));

    services.AddDbContext<InstaHelperDbContext>(opt => 
        opt.UseSqlServer(context.Configuration.GetConnectionString("InstaHelperConnection")));

    services.AddCoreServices(context.Configuration);
    services.AddHostedService<PollingService>();
})
.Build();

await host.RunAsync();