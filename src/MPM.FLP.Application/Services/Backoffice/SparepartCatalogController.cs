using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace MPM.FLP.Services.Backoffice
{
    public class SparepartCatalogController : FLPAppServiceBase, ISparepartCatalogController
    {
        private readonly ProductCatalogAppService _appService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SparepartCatalogController(ProductCatalogAppService appService, IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("/api/services/app/backoffice/SparepartCatalog/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/SparepartCatalog/getByID")]
        public ProductCatalogs GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        #region Main View
        private async Task<ProductCatalogAttachments> InsertToAzureCreate([FromForm]IFormFile file, [FromForm]ProductCatalogs model, [FromForm]string nama)
        {
            ProductCatalogAttachments attachments = new ProductCatalogAttachments();

            var configuration = new AzureController().GetConnectionToAzure();
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("productcatalog");

                var path = Path.GetExtension(file.FileName);

                string namaFile = nama + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + path;

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(namaFile);

                //await cloudBlockBlob.UploadFromFileAsync(file.FileName);
                using (Stream stream = file.OpenReadStream())
                {
                    stream.Position = 0;
                    await cloudBlockBlob.UploadFromStreamAsync(stream);
                }

                attachments.Id = Guid.NewGuid();
                attachments.CreationTime = DateTime.Now;
                attachments.CreatorUsername = "admin";
                attachments.LastModifierUsername = "admin";
                attachments.DeleterUsername = null;
                attachments.ProductCatalogId = model.Id;
                attachments.Order = "";
                attachments.DeleterUsername = "";
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpPost("/api/services/app/backoffice/SparepartCatalog/uploadDocument")]
        public async Task<ProductCatalogs> UploadDocument(Guid id, string SparepartDocUrl)
        {
            //AzureController azureController = new AzureController();
            ProductCatalogs model = _appService.GetById(id);

            model.SparepartDocUrl = SparepartDocUrl;
            _appService.Update(model);

            /*
            if(files.Count() > 0)
            {
                //model.SparepartDocUrl = await azureController.InsertAndGetUrlAzure(files.First(), id.ToString(), "DOC", "sparepartcatalog");
            }
            */

            return model;
        }

        [HttpPut("/api/services/app/backoffice/SparepartCatalog/update")]
        public async Task<ProductCatalogs> Edit(ProductCatalogs model, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                foreach (var file in files)
                {
                    model.ProductCatalogAttachments.Add(await InsertToAzureCreate(file, model, "IMG"));
                }
                
                if(model.ProductCatalogAttachments.Count > 0)
                    model.FeaturedImageUrl = model.ProductCatalogAttachments.Where(x => x.StorageUrl.Contains("IMG")).FirstOrDefault().StorageUrl;

                _appService.Update(model);
            }

            return model;
        }

        [HttpDelete("/api/services/app/backoffice/SparepartCatalog/deleteAttachment")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        #endregion

        [HttpGet("/api/services/app/backoffice/SparepartCatalog/downloadFile")]
        public IActionResult DownloadFile(Guid id)
        {
            try {
            var model = _appService.GetById(id);

            byte[] fileData = null;

            if (model != null)
            {
                using (var client = new WebClient())
                {
                    if (!string.IsNullOrEmpty(model.SparepartDocUrl))
                    {
                        fileData = client.DownloadData(model.SparepartDocUrl);
                    }
                }
            }

            MemoryStream stream = new MemoryStream(fileData);
            
            return new FileStreamResult(stream,"application/pdf") 
            { 
                FileDownloadName = "fileDownload"
            }; 
            } catch(Exception x){
                return null;
            } 
        }

    }
}