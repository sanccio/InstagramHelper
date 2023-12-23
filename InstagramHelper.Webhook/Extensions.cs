using Microsoft.Extensions.Options;

namespace InstagramHelper.Webhook
{
    public static class Extensions
    {
        public static T GetConfiguration<T>(this IServiceProvider serviceProvider)
        where T : class
        {
            var o = serviceProvider.GetService<IOptions<T>>();
            return o is null 
                ? throw new ArgumentNullException(nameof(T)) 
                : o.Value;
        }

        public static ControllerActionEndpointConventionBuilder MapBotWebhookRoute<T>(
            this IEndpointRouteBuilder endpoints,
            string route)
        {
            var controllerName = typeof(T).Name.Replace("Controller", "", StringComparison.Ordinal);
            var actionName = typeof(T).GetMethods()[0].Name;

            return endpoints.MapControllerRoute(
                name: "bot_webhook",
                pattern: route,
                defaults: new { controller = controllerName, action = actionName });
        }
    }
}
