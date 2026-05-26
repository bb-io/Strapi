using Apps.Strapi.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Apps.Strapi.Utils;

public static class AuthenticationCredentialsProviderExtensions
{
    public static string GetUrl(this IEnumerable<AuthenticationCredentialsProvider> credentialsProviders)
    {
        var urlCredentialProvider = credentialsProviders.Get(CredNames.BaseUrl);
        if(string.IsNullOrEmpty(urlCredentialProvider.Value))
            throw new PluginApplicationException($"Credential '{CredNames.BaseUrl}' is missing or empty.");
        
        var url = urlCredentialProvider.Value.TrimEnd('/');
        if (!url.Contains("/api"))
        {
            return $"{url}/api";
        }
        
        return url;
    }
}