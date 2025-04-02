using Apps.Strapi.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Strapi.Api;

public class ApiClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    : BlackBirdRestClient(new()
    {
        BaseUrl = new(authenticationCredentialsProviders.Get(CredsNames.Url).Value),
        ThrowOnAnyError = false
    })
{
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        throw new Exception(response.Content!);
    }
}