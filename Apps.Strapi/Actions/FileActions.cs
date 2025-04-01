using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Strapi.Actions;

[ActionList]
public class FileActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Action", Description = "Action description")] //TODO: fill description
    public async Task Action()
    {
        throw new NotImplementedException();
    }
}
