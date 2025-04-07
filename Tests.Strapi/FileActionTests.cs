using Apps.Strapi.Actions;
using Tests.Strapi.Base;
using Apps.Strapi.Models.Requests.Files;

namespace Tests.Strapi
{
    [TestClass]
    public class FileActionTests : TestBase
    {
        [TestMethod]
        public async Task GetFiles_ShouldReturnFiles()
        {
            var fileAction = new FileActions(InvocationContext);

            var files = await fileAction.GetFiles();

        }

        [TestMethod]
        public async Task GetFile_ShouldReturnFile()
        {
            var fileAction = new FileActions(InvocationContext);

            var file = await fileAction.GetFile(new GetFileRequest() { Id = 1});
        }

        [TestMethod]
        public async Task UploadFile_ShouldUploadFile()
        {
            var fileAction = new FileActions(InvocationContext);

            await fileAction.UploadFiles(new UploadFilesRequest());
        }

        [TestMethod]
        public async Task UploadFileInfo_ShouldUploadFileInfo()
        {
            var fileAction = new FileActions(InvocationContext);

            await fileAction.UploadFileInfo(new UploadFileInfoRequest());
        }

        [TestMethod]
        public async Task DeleteFile_ShouldDeleteFile()
        {
            var fileAction = new FileActions(InvocationContext);

            await fileAction.DeleteFile(new DeleteFileRequest() { Id = 1 });
        }
    }
}
