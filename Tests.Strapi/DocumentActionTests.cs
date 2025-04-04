using Apps.Strapi.Actions;
using Models.Requests;
using Tests.Strapi.Base;

namespace Tests.Strapi
{
    [TestClass]
    public class DocumentActionTests : TestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            // Initialize any required services or dependencies here
            // For example, you might want to set up a mock Strapi client
        }
        [TestMethod]
        public async Task GetDocuments_ShouldReturnDocuments()
        {
            var documentAction = new DocumentActions(InvocationContext);

            await documentAction.GetDocuments(new GetDocumentRequest
            {
                ApiId = "about"
            });
        }

        [TestMethod]
        public async Task CreateDocument_ShouldCreateDocument()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task UpdateDocument_ShouldUpdateDocument()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task DeleteDocument_ShouldDeleteDocument()
        {
            Assert.Fail();
        }
    }
}
