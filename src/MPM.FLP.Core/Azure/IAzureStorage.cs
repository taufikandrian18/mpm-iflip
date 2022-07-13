using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;

namespace MPM.FLP.Azure
{
    public interface IAzureStorage
    {
        Task<byte[]> GetBlobContentBytes(string containerName, string blobName);
        Task<BlobContentInfo> UploadBlob(string containerName, string blobName, Stream contentStream, bool overwrite = false);
        Task<string> UploadBlobAndGetUrl(string containerName, string blobName, Stream contentStream, bool overwrite = false);
        Task<string> UploadTempBlobAndGetUrl(string blobName, Stream contentStream, bool overwrite = false);
    }
}
