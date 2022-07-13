using Microsoft.AspNetCore.Mvc;
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

namespace MPM.FLP.Services.Backoffice
{
    public class KategoriProductController : FLPAppServiceBase, IKategoriProductController
    {
        private readonly ProductCategoryAppService _appService;

        public KategoriProductController(ProductCategoryAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/KategoriProduct/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/KategoriProduct/getByID")]
        public ProductCategories GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/KategoriProduct/create")]
        public async Task<ProductCategories> Create([FromForm]ProductCategories model, [FromForm]IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";
                model.IconUrl = "";
                if (files.Count() > 0)
                {
                    AzureController azureController = new AzureController();
                    //model.IconUrl = await InsertToAzure(files.FirstOrDefault(), model);
                    model.IconUrl = await azureController.InsertAndGetUrlAzure(files.FirstOrDefault(), model.Id.ToString(), "IMG", "kategoriproduct");
                }

                _appService.Create(model);
                
            };

            return model;
        }

        private async Task<string> InsertToAzure(IFormFile file, ProductCategories model)
        {
            string url = "";
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);
            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("kategoriproduct");

                var path = Path.GetExtension(file.FileName);
                string namaFile = "IMG_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + path;

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

        [HttpPut("/api/services/app/backoffice/KategoriProduct/update")]
        public async Task<ProductCategories> Edit(ProductCategories model, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                if (files.Count() > 0)
                {
                    model.IconUrl = await InsertToAzure(files.FirstOrDefault(), model);
                }

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/KategoriProduct/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

    }
}