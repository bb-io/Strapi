using Apps.Strapi.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Strapi;

public class Invocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected StrapiClient Client { get; }
    protected IFileManagementClient FileManagementClient { get; }

    public Invocable(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
    {
        Client = new(Creds);
        FileManagementClient = fileManagementClient;
    }
}
