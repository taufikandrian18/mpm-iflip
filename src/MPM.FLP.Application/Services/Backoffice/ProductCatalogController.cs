using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using OfficeOpenXml;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml.Style;
using OfficeOpenXml.DataValidation;

namespace MPM.FLP.Services.Backoffice
{
    public class ProductCatalogController : FLPAppServiceBase, IProductCatalogController
    {
        private readonly ProductCatalogAppService _appService;
        private readonly ProductCategoryAppService _categoryAppService;
        private readonly ProductColorVariantsAppService _colorVariantsAppService;
        private readonly ProductAccesoriesAppService _accesoriesAppService;
        private readonly ProductFeaturesAppService _featuresAppService;
        private readonly ProductPriceAppService _pricesAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductCatalogController(ProductCatalogAppService appService, ProductCategoryAppService categoryAppService, ProductColorVariantsAppService colorVariantsAppService, ProductAccesoriesAppService accesoriesAppService, ProductFeaturesAppService featuresAppService, ProductPriceAppService pricesAppService, IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _categoryAppService = categoryAppService;
            _colorVariantsAppService = colorVariantsAppService;
            _accesoriesAppService = accesoriesAppService;
            _featuresAppService = featuresAppService;
            _pricesAppService = pricesAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("/api/services/app/backoffice/ProductCatalogs/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAllBackoffice();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.ProductCode.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ProductCatalogs/getByID")]
        public ProductCatalogs GetByIDBackoffice(Guid guid)
        {
            return _appService.GetByIdAdmin(guid);
        }

        #region Main View
        [HttpGet("/api/services/app/backoffice/ProductCatalogs/getCategories")]
        public List<ProductCategories> GetCategories()
        {
            return _categoryAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToList();
        }

        [HttpPost("/api/services/app/backoffice/ProductCatalogs/create")]
        public async Task<ProductCatalogs> Create([FromForm]ProductCatalogsVM data, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> tvcvideo1, [FromForm]IEnumerable<IFormFile> tvcvideo2, [FromForm]IEnumerable<IFormFile> tvcvideo3)
        {
            ProductCatalogs model = new ProductCatalogs();

            model.Id = Guid.NewGuid();
            model.ProductCategoryId = data.ProductCategoryId;
            model.IsPublished = data.IsPublished;
            model.Order = data.Order;
            model.Title = data.Title;
            model.SparepartDocUrl = data.SparepartDocUrl;
            model.PanjangLebarTinggi = data.PanjangLebarTinggi;
            model.JarakSumbuRoda = data.JarakSumbuRoda;
            model.JarakTerendahkeTanah = data.JarakTerendahkeTanah;
            model.BeratKosong = data.BeratKosong;
            model.KapasitasTangkiBahanBakar = data.KapasitasTangkiBahanBakar;
            model.TipeMesin = data.TipeMesin;
            model.VolumeLangkah = data.VolumeLangkah;
            model.SistemPendingin = data.SistemPendingin;
            model.SistemSuplaiBahanBakar = data.SistemSuplaiBahanBakar;
            model.DiameterLangkah = data.DiameterLangkah;
            model.TipeTransmisi = data.TipeTransmisi;
            model.PerbandinganKompresi = data.PerbandinganKompresi;
            model.DayaMaksimum = data.DayaMaksimum;
            model.TorsiMaksimum = data.TorsiMaksimum;
            model.PolaPengoperanGigi = data.PolaPengoperanGigi;
            model.TipeStarter = data.TipeStarter;
            model.TipeKopling = data.TipeKopling;
            model.KapasitasMinyakPelumas = data.KapasitasMinyakPelumas;
            model.TipeRangka = data.TipeRangka;
            model.UkuranBanDepan = data.UkuranBanDepan;
            model.UkuranBanBelakang = data.UkuranBanBelakang;
            model.TipeRemDepan = data.TipeRemDepan;
            model.TipeRemBelakang = data.TipeRemBelakang;
            model.TipeSuspensiDepan = data.TipeSuspensiDepan;
            model.TipeSuspensiBelakang = data.TipeSuspensiBelakang;
            model.TipeBaterai = data.TipeBaterai;
            model.SistemPengapian = data.SistemPengapian;
            model.TipeBateraiAki = data.TipeBateraiAki;
            model.TipeBusi = data.TipeBusi;
            model.ProductCode = data.ProductCode;
            model.CreationTime = DateTime.Now;
            model.CreatorUsername = "admin";
            model.LastModifierUsername = "admin";
            model.LastModificationTime = DateTime.Now;
            model.DeleterUsername = "";
            model.FeaturedImageUrl = "";

            foreach (var file in files)
            {
                model.ProductCatalogAttachments.Add(await InsertToAzureCreate(file, model, "IMG"));
            }
            if (model.ProductCatalogAttachments.Count > 0)
                model.FeaturedImageUrl = model.ProductCatalogAttachments.First().StorageUrl;
            foreach (var file in tvcvideo1)
            {
                model.ProductCatalogAttachments.Add(await InsertToAzureCreate(file, model, "TVCVIDEO1"));
            }
            foreach (var file in tvcvideo2)
            {
                model.ProductCatalogAttachments.Add(await InsertToAzureCreate(file, model, "TVCVIDEO2"));
            }
            foreach (var file in tvcvideo3)
            {
                model.ProductCatalogAttachments.Add(await InsertToAzureCreate(file, model, "TVCVIDEO3"));
            }

            _appService.Create(model);

            return model;
        }

        [HttpPost("/api/services/app/backoffice/ProductCatalogs/import")]
        public async Task<string> Import([FromForm]IEnumerable<IFormFile> excel)
        {
            var file = excel.FirstOrDefault();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["ProductCatalogs"];
                    var rowCount = worksheet.Dimension.Rows;

                    int order = 1;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var category = worksheet.Cells[row, 2].Value.ToString().ToLower().Trim();
                        var categoryId = _categoryAppService.GetAll().FirstOrDefault(x => x.Name.ToLower() == category).Id;

                        bool tampil = false;
                        var tampilkan = worksheet.Cells[row, 3].Value.ToString().Trim();
                        if (worksheet.Cells[row, 3].Value.ToString().ToLower().Trim() == "ya")
                        {
                            tampil = true;
                        }

                        ProductCatalogs productCatalogs = new ProductCatalogs
                        {
                            Id = Guid.NewGuid(),
                            CreationTime = DateTime.Now,
                            CreatorUsername = "admin",
                            LastModifierUsername = "admin",
                            LastModificationTime = DateTime.Now,
                            DeleterUsername = "",
                            ProductCategoryId = categoryId,
                            IsPublished = tampil,
                            Title = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            Order = order,
                            PanjangLebarTinggi = worksheet.Cells[row, 4].Value.ToString().Trim(),
                            JarakSumbuRoda = worksheet.Cells[row, 5].Value.ToString().Trim(),
                            JarakTerendahkeTanah = worksheet.Cells[row, 6].Value.ToString().Trim(),
                            BeratKosong = worksheet.Cells[row, 7].Value.ToString().Trim(),
                            KapasitasTangkiBahanBakar = worksheet.Cells[row, 8].Value.ToString().Trim(),
                            TipeMesin = worksheet.Cells[row, 9].Value.ToString().Trim(),
                            VolumeLangkah = worksheet.Cells[row, 10].Value.ToString().Trim(),
                            SistemPendingin = worksheet.Cells[row, 11].Value.ToString().Trim(),
                            SistemSuplaiBahanBakar = worksheet.Cells[row, 12].Value.ToString().Trim(),
                            DiameterLangkah = worksheet.Cells[row, 13].Value.ToString().Trim(),
                            TipeTransmisi = worksheet.Cells[row, 14].Value.ToString().Trim(),
                            PerbandinganKompresi = worksheet.Cells[row, 15].Value.ToString().Trim(),
                            DayaMaksimum = worksheet.Cells[row, 16].Value.ToString().Trim(),
                            TorsiMaksimum = worksheet.Cells[row, 17].Value.ToString().Trim(),
                            PolaPengoperanGigi = worksheet.Cells[row, 18].Value.ToString().Trim(),
                            TipeStarter = worksheet.Cells[row, 19].Value.ToString().Trim(),
                            TipeKopling = worksheet.Cells[row, 20].Value.ToString().Trim(),
                            KapasitasMinyakPelumas = worksheet.Cells[row, 21].Value.ToString().Trim(),
                            TipeRangka = worksheet.Cells[row, 22].Value.ToString().Trim(),
                            UkuranBanDepan = worksheet.Cells[row, 23].Value.ToString().Trim(),
                            UkuranBanBelakang = worksheet.Cells[row, 24].Value.ToString().Trim(),
                            TipeRemDepan = worksheet.Cells[row, 25].Value.ToString().Trim(),
                            TipeRemBelakang = worksheet.Cells[row, 26].Value.ToString().Trim(),
                            TipeSuspensiDepan = worksheet.Cells[row, 27].Value.ToString().Trim(),
                            TipeSuspensiBelakang = worksheet.Cells[row, 28].Value.ToString().Trim(),
                            TipeBaterai = worksheet.Cells[row, 29].Value.ToString().Trim(),
                            SistemPengapian = worksheet.Cells[row, 30].Value.ToString().Trim(),
                            TipeBateraiAki = worksheet.Cells[row, 31].Value.ToString().Trim(),
                            TipeBusi = worksheet.Cells[row, 32].Value.ToString().Trim()
                        };

                        _appService.Create(productCatalogs);
                        order++;
                    }
                    return "Success Import";
                }
            }
            return "Something went wrong";
        }

        private async Task<ProductCatalogAttachments> InsertToAzureCreate(IFormFile file, ProductCatalogs model, string nama)
        {
            ProductCatalogAttachments attachments = new ProductCatalogAttachments();

            var configuration = new AzureController().GetConnectionToAzure();

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


        [HttpPut("/api/services/app/backoffice/ProductCatalogs/update")]
        public async Task<ProductCatalogs> Edit([FromForm]ProductCatalogsVM data, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> tvcvideo1, [FromForm]IEnumerable<IFormFile> tvcvideo2, [FromForm]IEnumerable<IFormFile> tvcvideo3)
        {
            ProductCatalogs model = _appService.GetByIdAdmin(data.Id);
            model.ProductCategoryId = data.ProductCategoryId;
            model.IsPublished = data.IsPublished;
            model.Order = data.Order;
            model.Title = data.Title;
            model.SparepartDocUrl = data.SparepartDocUrl;
            model.PanjangLebarTinggi = data.PanjangLebarTinggi;
            model.JarakSumbuRoda = data.JarakSumbuRoda;
            model.JarakTerendahkeTanah = data.JarakTerendahkeTanah;
            model.BeratKosong = data.BeratKosong;
            model.KapasitasTangkiBahanBakar = data.KapasitasTangkiBahanBakar;
            model.TipeMesin = data.TipeMesin;
            model.VolumeLangkah = data.VolumeLangkah;
            model.SistemPendingin = data.SistemPendingin;
            model.SistemSuplaiBahanBakar = data.SistemSuplaiBahanBakar;
            model.DiameterLangkah = data.DiameterLangkah;
            model.TipeTransmisi = data.TipeTransmisi;
            model.PerbandinganKompresi = data.PerbandinganKompresi;
            model.DayaMaksimum = data.DayaMaksimum;
            model.TorsiMaksimum = data.TorsiMaksimum;
            model.PolaPengoperanGigi = data.PolaPengoperanGigi;
            model.TipeStarter = data.TipeStarter;
            model.TipeKopling = data.TipeKopling;
            model.KapasitasMinyakPelumas = data.KapasitasMinyakPelumas;
            model.TipeRangka = data.TipeRangka;
            model.UkuranBanDepan = data.UkuranBanDepan;
            model.UkuranBanBelakang = data.UkuranBanBelakang;
            model.TipeRemDepan = data.TipeRemDepan;
            model.TipeRemBelakang = data.TipeRemBelakang;
            model.TipeSuspensiDepan = data.TipeSuspensiDepan;
            model.TipeSuspensiBelakang = data.TipeSuspensiBelakang;
            model.TipeBaterai = data.TipeBaterai;
            model.SistemPengapian = data.SistemPengapian;
            model.TipeBateraiAki = data.TipeBateraiAki;
            model.TipeBusi = data.TipeBusi;
            model.ProductCode = data.ProductCode;
            model.LastModifierUsername = "admin";
            model.LastModificationTime = DateTime.Now;

            foreach (var file in files)
            {
                model.ProductCatalogAttachments.Add(await InsertToAzureCreate(file, model, "IMG"));
            }
            foreach (var file in tvcvideo1)
            {
                model.ProductCatalogAttachments.Add(await InsertToAzureCreate(file, model, "TVCVIDEO1"));
            }
            foreach (var file in tvcvideo2)
            {
                model.ProductCatalogAttachments.Add(await InsertToAzureCreate(file, model, "TVCVIDEO2"));
            }
            foreach (var file in tvcvideo3)
            {
                model.ProductCatalogAttachments.Add(await InsertToAzureCreate(file, model, "TVCVIDEO3"));
            }
            if (model.ProductCatalogAttachments.Count > 0)
                model.FeaturedImageUrl = model.ProductCatalogAttachments.Where(x => x.StorageUrl.Contains("IMG")).FirstOrDefault().StorageUrl;

            _appService.Update(model);

            return model;
        }

        [HttpDelete("/api/services/app/backoffice/ProductCatalogs/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        #endregion

        #region Spesifikasi
        [HttpGet("/api/services/app/backoffice/ProductCatalogs/spesifikasi")]
        public ProductCategories Spesifikasi(Guid id)
        {
            return _categoryAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Id == id).FirstOrDefault();
        }
        #endregion


        private async Task<string> InsertAndGetUrlAzure(IFormFile file, string id, string nama, string container)
        {
            string url = "";

            var configuration = new AzureController().GetConnectionToAzure();
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

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

        #region Varian Warna
        [HttpGet("/api/services/app/backoffice/ProductCatalogs/warna")]
        public BaseResponse VarianWarna([FromQuery] Pagination request, Guid id)
        {
            request = Paginate.Validate(request);

            var query = _colorVariantsAppService.GetAll().Where(x => x.ProductCatalogId == id);

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.Price.ToString() == request.Query
                );
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpPost("/api/services/app/backoffice/ProductCatalogs/createWarna")]
        public async Task<ProductColorVariants> CreateVarianWarna([FromForm]ProductCatalogWarnaVM model, [FromForm]IEnumerable<IFormFile> files, [FromForm]string picker)
        {
            ProductColorVariants colorVariants = new ProductColorVariants();
            colorVariants.Id = Guid.NewGuid();
            colorVariants.Title = model.Title;
            colorVariants.CreationTime = DateTime.Now;
            colorVariants.CreatorUsername = "admin";
            colorVariants.LastModifierUsername = "admin";
            colorVariants.LastModificationTime = DateTime.Now;
            colorVariants.DeleterUsername = "";
            colorVariants.ProductCatalogId = model.ProductId;
            colorVariants.ImageUrl = "";
            colorVariants.HexColor = picker;

            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    colorVariants.ImageUrl = await InsertAndGetUrlAzure(file, colorVariants.Id.ToString(), "IMG", "productcatalogcolor");
                }
            }

            _colorVariantsAppService.Create(colorVariants);

            return colorVariants;
        }

        [HttpPut("/api/services/app/backoffice/ProductCatalogs/editWarna")]
        [ValidateAntiForgeryToken]
        public async Task<ProductColorVariants> EditVarianWarna([FromForm]ProductCatalogWarnaEditVM model, IEnumerable<IFormFile> files, string picker)
        {
            ProductColorVariants colorVariants = _colorVariantsAppService.GetById(model.guid);

            colorVariants.Title = model.Title;
            colorVariants.HexColor = picker;
            colorVariants.LastModifierUsername = "admin";
            colorVariants.LastModificationTime = DateTime.Now;

            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    colorVariants.ImageUrl = await InsertAndGetUrlAzure(file, colorVariants.Id.ToString(), "IMG", "productcatalogcolor");
                }
            }

            _colorVariantsAppService.Update(colorVariants);

            return colorVariants;
        }

        [HttpDelete("/api/services/app/backoffice/ProductCatalogs/destroyWarna")]
        public String DestroyWarna(Guid guid)
        {
            _colorVariantsAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        #endregion

        #region Fitur Utama
        [HttpGet("/api/services/app/backoffice/ProductCatalogs/fiturUtama")]
        public BaseResponse FiturUtama([FromQuery] Pagination request, Guid id)
        {
            request = Paginate.Validate(request);

            var query = _featuresAppService.GetAll().Where(x => x.ProductCatalogId == id);

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.Description.ToString().Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpPost("/api/services/app/backoffice/ProductCatalogs/createFiturUtama")]
        public async Task<ProductFeatures> CreateFiturUtama([FromForm]ProductCatalogFiturUtamaVM model, [FromForm]IEnumerable<IFormFile> files)
        {
            ProductFeatures item = new ProductFeatures();
            item.Id = Guid.NewGuid();
            item.Title = model.Title;
            item.Description = model.TipeMesin;
            item.CreationTime = DateTime.Now;
            item.CreatorUsername = "admin";
            item.LastModifierUsername = "admin";
            item.LastModificationTime = DateTime.Now;
            item.DeleterUsername = "";
            item.ProductCatalogId = model.ProductId;
            item.ImageUrl = "";

            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    item.ImageUrl = await InsertAndGetUrlAzure(file, item.Id.ToString(), "IMG", "productcatalogmainfeature");
                }
            }

            _featuresAppService.Create(item);

            return item;
        }

        [HttpPut("/api/services/app/backoffice/ProductCatalogs/editFiturUtama")]
        public async Task<ProductFeatures> EditFiturUtama([FromForm]ProductCatalogFiturUtamaEditVM model, IEnumerable<IFormFile> files)
        {
            ProductFeatures item = _featuresAppService.GetById(model.guid);

            item.Title = model.Title;
            item.Description = model.TipeMesin;
            item.LastModifierUsername = "admin";
            item.LastModificationTime = DateTime.Now;

            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    item.ImageUrl = await InsertAndGetUrlAzure(file, item.Id.ToString(), "IMG", "productcatalogmainfeature");
                }
            }

            _featuresAppService.Update(item);
            return item;
        }

        [HttpDelete("/api/services/app/backoffice/ProductCatalogs/destroyFiturUtama")]
        public String DestroyFiturUtama(Guid guid)
        {
            _featuresAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
        #endregion

        #region Aksesoris
        [HttpGet("/api/services/app/backoffice/ProductCatalogs/aksesoris")]
        public BaseResponse Aksesoris([FromQuery] Pagination request, Guid id)
        {
            request = Paginate.Validate(request);

            var query = _accesoriesAppService.GetAll().Where(x => x.ProductCatalogId == id);

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.Description.ToString().Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpPost("/api/services/app/backoffice/ProductCatalogs/createAksesoris")]
        public async Task<ProductAccesories> CreateAksesoris([FromForm]ProductCatalogAksesorisEditVM model, [FromForm]IEnumerable<IFormFile> files)
        {
            ProductAccesories item = new ProductAccesories();
            item.Id = Guid.NewGuid();
            item.Title = model.Title;
            item.CreationTime = DateTime.Now;
            item.CreatorUsername = "admin";
            item.LastModifierUsername = "admin";
            item.LastModificationTime = DateTime.Now;
            item.DeleterUsername = "";
            item.ProductCatalogId = model.ProductCatalogId;
            item.ImageUrl = "";
            item.Price = model.PanjangLebarTinggi;
            item.Description = model.DiameterLangkah;
            item.AccessoriesCode = model.ProductCode;

            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    item.ImageUrl = await InsertAndGetUrlAzure(file, item.Id.ToString(), "IMG", "productcatalogaccessories");
                }
            }

            _accesoriesAppService.Create(item);
            return item;
        }

        [HttpPut("/api/services/app/backoffice/ProductCatalogs/editAksesoris")]
        public async Task<ProductAccesories> EditAksesoris([FromForm]ProductCatalogAksesorisEditVM model, IEnumerable<IFormFile> files)
        {
            ProductAccesories item = _accesoriesAppService.GetById(model.guid);

            item.Title = model.Title;
            item.Price = model.PanjangLebarTinggi;
            item.Description = model.DiameterLangkah;
            item.AccessoriesCode = model.ProductCode;
            item.LastModifierUsername = "admin";
            item.LastModificationTime = DateTime.Now;

            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    item.ImageUrl = await InsertAndGetUrlAzure(file, item.Id.ToString(), "IMG", "productcatalogaccessories");
                }
            }

            _accesoriesAppService.Update(item);
            
            return item;
        }

        [HttpDelete("/api/services/app/backoffice/ProductCatalogs/destroyAksesoris")]
        public String DestroyAksesoris(Guid guid)
        {
            _accesoriesAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
        #endregion

        #region Varian Harga
        [HttpGet("/api/services/app/backoffice/ProductCatalogs/harga")]
        public ProductPrices VariantHarga(Guid id)
        {
            return _pricesAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.ProductColorVariantId == id).FirstOrDefault();
        }

        [HttpGet("/api/services/app/backoffice/ProductCatalogs/downloadTemplate")]
        public ActionResult DownloadTemplate(Guid id)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Template Variant Harga.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Template Harga");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Kode Dealer";
                    workSheet.Cells[1, 2].Value = "Product Color Variant Id";
                    workSheet.Cells[1, 3].Value = "Harga";

                    //workSheet.Cells[2, 1].Value = "A0010";
                    //workSheet.Cells[2, 2].Value = "32d7c6dc-d9d7-48da-b6c5-476891228351";
                    //workSheet.Cells[2, 3].Value = "1000000";

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();

                    var validation = workSheet.DataValidations.AddListValidation("B2:B200");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    validation.ErrorTitle = "An invalid value was entered";
                    validation.Error = "Select a value from the list";

                    var workSheetColor = package.Workbook.Worksheets.Add("Colors");
                    workSheetColor.Cells[1, 1].Value = "Product Color Name";
                    workSheetColor.Cells[1, 2].Value = "Product Color Variant Id";

                    var colorVariants = _appService.GetByIdAdmin(id).ProductColorVariants;
                    int i = 2;
                    foreach (var colorVariant in colorVariants)
                    {
                        workSheetColor.Cells[i, 1].Value = colorVariant.Title;
                        workSheetColor.Cells[i, 2].Value = colorVariant.Id;
                        validation.Formula.Values.Add(colorVariant.Title);
                        i++;
                    }

                    workSheetColor.Column(1).AutoFit();
                    workSheetColor.Column(2).AutoFit();
                    package.Save();
                }
            }
            catch (Exception ex)
            {

            }

            stream.Position = 0;

            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = excelName
            };
        }

        [HttpPost("/api/services/app/backoffice/ProductCatalogs/createHarga")]
        public async Task<ProductPrices> CreateVarianHarga([FromForm]IEnumerable<IFormFile> files)
        {
            if (files.Count() > 0)
            {
                var file = files.FirstOrDefault();
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Template"];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var kodeDealerMPM = worksheet.Cells[row, 1].Value.ToString().Trim();
                            var productColorVariantId = Guid.Parse(worksheet.Cells[row, 2].Value.ToString().Trim());
                            var priceStr = worksheet.Cells[row, 3].Value.ToString().Trim();
                            int price = 0;

                            if (priceStr != "" || priceStr != null)
                            {
                                price = int.Parse(priceStr);
                            }

                            if (_pricesAppService.GetAll().Where(x => x.KodeDealerMPM == kodeDealerMPM && x.ProductColorVariantId == productColorVariantId).Count() > 0)
                            {
                                var attachment = _pricesAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == kodeDealerMPM && x.ProductColorVariantId == productColorVariantId);
                                attachment.Price = price;
                                _pricesAppService.Update(attachment);

                                return attachment;
                            }
                            else
                            {
                                ProductPrices attachment = new ProductPrices
                                {
                                    Id = Guid.NewGuid(),
                                    KodeDealerMPM = kodeDealerMPM,
                                    ProductColorVariantId = productColorVariantId,
                                    Price = price,
                                    CreationTime = DateTime.Now,
                                    CreatorUsername = "admin",
                                    LastModifierUsername = "admin",
                                    LastModificationTime = DateTime.Now,
                                    DeleterUsername = ""
                                };
                                _pricesAppService.Create(attachment);

                                return attachment;
                            }

                        }
                    }
                }
            }
            
            return null;
        }

        public String EditVarianHarga(ProductPrices productPrices)
        {
            _pricesAppService.Update(productPrices);
            return "Success";
        }
        #endregion
    }
}