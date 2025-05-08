using Apps.Strapi.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Strapi.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups =>
    [
        new()
        {
            Name = "Developer API key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties =
            [
                new(CredsNames.BaseUrl)
                {
                    DisplayName = "Base URL",
                    Sensitive = false
                },
                new(CredsNames.ApiToken) 
                { 
                    DisplayName = "API Token", 
                    Sensitive = true
                }
            ]
        }
    ];
    
    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values) => values.Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value)).ToList();
}
