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
            var result = await client.ExecuteAsync(new RestRequest("/api"));
            
            if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var errorDto = JsonConvert.DeserializeObject<ErrorDto>(result.Content!)!;
                var isValid = errorDto.Error.Status == 404 && errorDto.Error.Name == "NotFoundError";
                return new()
                {
                    IsValid = isValid,
                    Message = isValid ? "It's not possible to check if the api token is valid, but at least the URL is correct" : result.Content
                };
            }

            return new()
            {
                IsValid = false,
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