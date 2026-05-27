using System.Net;
using Apps.Strapi.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Strapi.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, CancellationToken cancellationToken)
    {
        var client = new StrapiClient(authenticationCredentialsProviders);
        var response = await client.ExecuteAsync(new RestRequest("/i18n/locales"), cancellationToken);
            
        var isValid = response.StatusCode != HttpStatusCode.Unauthorized && response.StatusCode != HttpStatusCode.Forbidden
                        && response.StatusCode != HttpStatusCode.NotFound && (int)response.StatusCode != 0;
        
        return new()
        {
            IsValid = isValid,
            Message = isValid ? "Connection successful." : $"Connection failed with status code: {response.StatusCode} and message: {response.Content}"
        };
    }
}