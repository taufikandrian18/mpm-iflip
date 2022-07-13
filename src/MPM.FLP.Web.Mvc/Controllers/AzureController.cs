using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MPM.FLP.Configuration;

namespace MPM.FLP.Web.Mvc.Controllers
{
    public class AzureController 
    {
        public IConfigurationRoot GetConnectionToAzure()
        {
            //For publish connection
            return AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            //For local connection
            //return AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());
        }

        public async Task<string> InsertAndGetUrlAzure(IFormFile file, string id, string nama, string container)
        {
            string url = "";

            var configuration = GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(container);

                var path = Path.GetExtension(file.FileName);

                string namaFile = nama + "_" + id + "_" + DateTime.Now.ToString("yyyyMMdd") + path;

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(namaFile);

                //await cloudBlockBlob.UploadFromFileAsync(file.FileName);
                using (Stream stream = file.OpenReadStream())
                {
                    stream.Position = 0;
                    await cloudBlockBlob.UploadFromStreamAsync(stream);
                }

                url = cloudBlockBlob.Uri.AbsoluteUri;
            }

            return url;
        }
    }
}