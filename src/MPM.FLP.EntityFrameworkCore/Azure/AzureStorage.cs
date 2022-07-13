using Abp.Dependency;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using MPM.FLP.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace MPM.FLP.Azure
{
    public sealed class AzureStorage : IAzureStorage, ISingletonDependency
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureStorage()
        {
            IConfigurationRoot configuration = AppConfigurations.Get();
            string connectionString = configuration.GetConnectionString(FLPConsts.AzureConnectionString);
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<byte[]> GetBlobContentBytes(string containerName, string blobName)
        {
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobDownloadInfo download = await container.GetBlobClient(blobName).DownloadAsync();
            using (MemoryStream ms = new MemoryStream())
            {
                await download.Content.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public async Task<BlobContentInfo> UploadBlob(string containerName, string blobName, Stream contentStream, bool overwrite = false)
        {
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(blobName);
            BlobContentInfo blobContentInfo = await blob.UploadAsync(contentStream, overwrite);
            return blobContentInfo;
        }

        public async Task<string> UploadBlobAndGetUrl(string containerName, string blobName, Stream contentStream, bool overwrite = false)
        {
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(blobName);
            await blob.UploadAsync(contentStream, overwrite);
            return blob.Uri.AbsoluteUri;
        }

        public async Task<string> UploadTempBlobAndGetUrl(string blobName, Stream contentStream, bool overwrite = false)
        {
            string containerName = "temp-blob-download";
            return await UploadBlobAndGetUrl(containerName, blobName, contentStream, overwrite);
        }
    }
}
