using Apps.Strapi.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Strapi.Api;

public class ApiRequest(string resource, Method method, IEnumerable<AuthenticationCredentialsProvider> creds)
    : BlackBirdRestRequest(resource, method, creds)
{
    protected override void AddAuth(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var apiToken = creds.Get(CredsNames.ApiToken).Value;
        this.AddHeader("Authorization", $"Bearer {apiToken}");
    }
}