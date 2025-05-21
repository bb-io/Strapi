using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Strapi.Utils;

public static class ExceptionExtensions
{
    public static void ThrowIfNullOrEmpty(this string? value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new PluginMisconfigurationException(
                $"The parameter '{parameterName}' cannot be null or empty. Please provide a valid value.");
        }
    }

    public static void ThrowIfNull(this object? value, string parameterName)
    {
        if (value == null)
        {
            throw new PluginMisconfigurationException(
                $"The parameter '{parameterName}' cannot be null. Please provide a valid value.");
        }
    }
}
