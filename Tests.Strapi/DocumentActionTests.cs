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
            var documentAction = new DocumentActions(InvocationContext, FileManager);

            var result = await documentAction.GetDocuments(new GetDocumentsRequest
            {
                ApiId = "articles"
            });

            Assert.AreNotEqual(0,result.Data.Count());
        }

        [TestMethod]
        public async Task GetDocument_ShouldReturnDocument()
        {
            var documentAction = new DocumentActions(InvocationContext, FileManager);

            var result = await documentAction.GetDocument(new GetDocumentRequest()
            {
                ApiId = "articles",
                DocumentId = "xp8871kg48cl3x0x9k35lxdw"
            });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task CreateDocument_ShouldCreateDocument()
        {
            var documentAction = new DocumentActions(InvocationContext,FileManager);

            var file = new Blackbird.Applications.Sdk.Common.Files.FileReference() { Name = "createdocument.json" };
            var result = await documentAction.CreateDocument(new CreateDocumentRequest
            {
                ApiId = "articles",
                File = file
            });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UpdateDocument_ShouldUpdateDocument()
        {
            var documentAction = new DocumentActions(InvocationContext, FileManager);
            var file = new Blackbird.Applications.Sdk.Common.Files.FileReference() { Name = "updatedocument.json" };

            var result = await documentAction.UpdateDocument(new UpdateDocumentRequest
            {
                ApiId = "articles",
                DocumentId= "xp8871kg48cl3x0x9k35lxdw",
                File = file
            });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeleteDocument_ShouldDeleteDocument()
        {
            var documentAction = new DocumentActions(InvocationContext, FileManager);

            await documentAction.DeleteDocument(new DeleteDocumentRequest
            {
                ApiId = "articles",
                DocumentId = "xp8871kg48cl3x0x9k35lxdw"
            });

            var result = await documentAction.GetDocument(new GetDocumentRequest
            {
                ApiId = "articles",
                DocumentId = "xp8871kg48cl3x0x9k35lxdw"
            });

            Assert.IsNull(result.DocumentId);
        }
    }
}
