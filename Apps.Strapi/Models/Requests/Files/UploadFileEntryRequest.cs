using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using System.Reflection;
using System.Xml;

namespace Apps.Strapi.Models.Requests.Files
{
    public class UploadFileEntryRequest 
    {

        [Display("Files", Description = "Files to upload.")]
        public FileReference[]? Files { get; set; } = Array.Empty<FileReference>();

        [Display("Path", Description = "The folder where the file(s) will be uploaded to (only supported on strapi-provider-upload-aws-s3).")]
        public string? Path { get; set; }

        [Display("Ref Id", Description = "The ID of the entry which the file(s) will be linked to.")]
        public string? RefId { get; set; }

        [Display("Ref", Description = "The unique ID(uid) of the model which the file(s) will be linked to ")]
        public string? Ref { get; set; }

        [Display("Source", Description = "The name of the plugin where the model is located.")]
        public string? Source { get; set; }

        [Display("Field", Description = "The field of the entry which the file(s) will be precisely linked to.")]
        public string? Field { get; set; }
    }
}
