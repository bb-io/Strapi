using Apps.Strapi.Actions;
using Tests.Strapi.Base;

namespace Tests.Strapi;

[TestClass]
public class ActionTests : TestBase
{
    [TestMethod]
    public async Task Dynamic_handler_works()
    {
        var actions = new Actions(InvocationContext);

        await actions.Action();
    }
}
