using Apps.Strapi.Api;
using Apps.Strapi.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Strapi.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, CancellationToken cancellationToken)
    {
        try
        {
            var client = new StrapiClient(authenticationCredentialsProviders);
            var result = await client.ExecuteWithErrorHandling(new RestRequest("/api/i18n/locales"));
            return new()
            {
                IsValid = true,
                Message = result.Content
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }
    }
}