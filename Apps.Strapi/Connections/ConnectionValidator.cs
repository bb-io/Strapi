using Apps.Strapi.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Strapi.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, CancellationToken cancellationToken)
    {
        try
        {
            var client = new StrapiClient(authenticationCredentialsProviders);
            var result = await client.ExecuteWithErrorHandling(new RestRequest());

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new()
                {
                    IsValid = true
                };
            }
            else
            {
                return new()
                {
                    IsValid = false,
                    Message = result.Content
                };
            }
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