using Apps.Strapi.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Strapi;

public class Invocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Credentials =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected StrapiClient Client { get; }

    public Invocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Credentials);
    }
}