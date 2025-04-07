using Apps.Strapi.Actions;
using Apps.Strapi.Models.Requests.Documents;
using Tests.Strapi.Base;

namespace Tests.Strapi
{
    [TestClass]
    public class DocumentActionTests : TestBase
    {

        [TestMethod]
        public async Task GetDocuments_ShouldReturnDocuments()
        {
            var documentAction = new DocumentActions(InvocationContext);

            await documentAction.GetDocuments(new GetDocumentsRequest
            {
                ApiId = "about"
            });
        }

        [TestMethod]
        public async Task GetDocument_ShouldReturnDocument()
        {
            var documentAction = new DocumentActions(InvocationContext);

            await documentAction.GetDocument(new GetDocumentRequest
            {
                ApiId = "about"
            });
        }

        [TestMethod]
        public async Task CreateDocument_ShouldCreateDocument()
        {
            var documentAction = new DocumentActions(InvocationContext);

            await documentAction.CreateDocument(new CreateDocumentRequest
            {
                ApiId = "about"
            });
        }

        [TestMethod]
        public async Task UpdateDocument_ShouldUpdateDocument()
        {
            var documentAction = new DocumentActions(InvocationContext);

            await documentAction.UpdateDocument(new UpdateDocumentRequest
            {
                ApiId = "about",
                Id=1
            });
        }

        [TestMethod]
        public async Task DeleteDocument_ShouldDeleteDocument()
        {
            var documentAction = new DocumentActions(InvocationContext);

            await documentAction.DeleteDocument(new DeleteDocumentRequest
            {
                ApiId = "about",
                Id = 1
            });
        }
    }
}
