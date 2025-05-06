using Apps.Strapi.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Strapi.Api;

public class StrapiClient : BlackBirdRestClient
{
    public StrapiClient(IEnumerable<AuthenticationCredentialsProvider> creds) : base(new()
    {
        BaseUrl = new(creds.Get(CredsNames.BaseUrl).Value.Trim('/')),
        ThrowOnAnyError = false
    })
    {
        var apiToken = creds.Get(CredsNames.ApiToken).Value;
        this.AddDefaultHeader("Authorization", $"Bearer {apiToken}");
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject(response.Content);
        var errorMessage = "";

        throw new PluginApplicationException(response.Content);
    }
}