using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Strapi.Base;

public class FileManager : IFileManagementClient
{
    private readonly string inputFolder;
    private readonly string outputFolder;

    public FileManager()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var projectDirectory = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent?.FullName
            ?? throw new DirectoryNotFoundException("Project directory not found.");


        var testFilesPath = Path.Combine(projectDirectory, "TestFiles");
        inputFolder = Path.Combine(testFilesPath, "Input");
        outputFolder = Path.Combine(testFilesPath, "Output");

        Directory.CreateDirectory(inputFolder);
        Directory.CreateDirectory(outputFolder);
    }


    public Task<Stream> DownloadAsync(FileReference reference)
    {
        var path = Path.Combine(inputFolder, reference.Name);
        Assert.IsTrue(File.Exists(path), $"File not found at: {path}");
        var bytes = File.ReadAllBytes(path);

        var stream = new MemoryStream(bytes);
        return Task.FromResult((Stream)stream);
    }

    public Task<FileReference> UploadAsync(Stream stream, string contentType, string fileName)
    {
        var path = Path.Combine(outputFolder, fileName);
        FileInfo fileInfo = new(path);
        fileInfo.Directory!.Create();
        using (var fileStream = File.Create(path))
        {
            stream.CopyTo(fileStream);
        }

        return Task.FromResult(new FileReference() { Name = fileName, ContentType = contentType });
    }
}

