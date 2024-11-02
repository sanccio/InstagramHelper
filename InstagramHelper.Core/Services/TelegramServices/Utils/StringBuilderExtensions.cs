using System.Text;

namespace InstagramHelper.Core.Services.TelegramServices.Utils;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendLineIfNotNullOrEmpty(this StringBuilder builder, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            builder.AppendLine(value);
        }

        return builder;
    }
}
