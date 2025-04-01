using Apps.Appname.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Appname.Connections;

public class ConnectionValidator: IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = new Client(authenticationCredentialsProviders);

            await client.ExecuteWithErrorHandling(new RestRequest());

            return new()
            {
                IsValid = true
            };
        } catch(Exception ex)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }

    }
}