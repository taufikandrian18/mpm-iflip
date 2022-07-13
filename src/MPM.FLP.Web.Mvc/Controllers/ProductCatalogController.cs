using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Controllers;
using MPM.FLP.Services;
using System.Collections.Generic;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using MPM.FLP.Configuration;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Hosting;
using Abp.AspNetCore.Mvc.Authorization;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class ProductCatalogController : FLPControllerBase
    {
        private readonly ProductCatalogAppService _appService;
        private readonly ProductCategoryAppService _categoryAppService;
        private readonly ProductColorVariantsAppService _colorVariantsAppService;
        private readonly ProductAccesoriesAppService _accesoriesAppService;
        private readonly ProductFeaturesAppService _featuresAppService;
        private readonly ProductPriceAppService _pricesAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductCatalogController(IHostingEnvironment hostingEnvironment, ProductCatalogAppService appService, ProductCategoryAppService categoryAppService, ProductColorVariantsAppService colorVariantsAppService, ProductAccesoriesAppService accesoriesAppService, ProductFeaturesAppService featuresAppService, ProductPriceAppService pricesAppService)
        {
            _appService = appService;
            _categoryAppService = categoryAppService;
            _colorVariantsAppService = colorVariantsAppService;
            _accesoriesAppService = accesoriesAppService;
            _featuresAppService = featuresAppService;
            _pricesAppService = pricesAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult BackToIndex()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            //if (!PermissionChecker.IsGranted("Menu.ProductCatalog"))
            //{
            //    //Alerts.Warning("You are not authorized to open product catalog!", "Alert");
            //    //return View();
            //    //throw new AbpAuthorizationException("You are not authorized to open product catalog!");
            //    throw new UserFriendlyException("Not Authorized", "You are not authorized to open product catalog!");
            //}
            //else
            TempData["alert"] = "";
            TempData["success"] = "";
            
            return View();
        }

        #region Main View
        //public IActionResult Create()
        //{
        //    ProductCatalogs model = new ProductCatalogs();
        //    return View(model);
        //}

        public IActionResult Create(ProductCatalogs model)
        {
            return View(model);
        }

        public IActionResult GetCategorys([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = _categoryAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCatalogs model, string submit, IEnumerable<IFormFile> excel, IEnumerable<IFormFile> files, IEnumerable<IFormFile> tvcvideo1, IEnumerable<IFormFile> tvcvideo2, IEnumerable<IFormFile> tvcvideo3)
        {
            if(excel.Count() > 0)
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
                        //TempData["alert"] = "Bahaya";
                        //return RedirectToAction("VarianHarga/" + model.Id);
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

                            ProductCatalogs productCatalogs = new ProductCatalogs {
                                Id = Guid.NewGuid(),
                                CreationTime = DateTime.Now,
                                CreatorUsername = this.User.Identity.Name,
                                LastModifierUsername = this.User.Identity.Name,
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
                            //if (_pricesAppService.GetAll().Where(x => x.KodeDealerMPM == attachment.KodeDealerMPM && x.ProductColorVariantId == attachment.ProductColorVariantId) != null)
                            //{
                            //    _pricesAppService.Update(attachment);
                            //    //TempData["alert"] = "Data dengan kode dealer " + attachment.KodeDealerMPM + " dan color variant id " + attachment.ProductColorVariantId;
                            //    //TempData["success"] = "";
                            //    //return RedirectToAction("VarianHarga/" + model.Id);
                            //}
                            //else
                            //{
                            //    _pricesAppService.Create(attachment);
                            //}

                        }
                    }
                }
            }
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }
                if (model.ProductCategoryId == null)
                {
                    TempData["alert"] = "Kategori produk masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }

                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
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

                ViewBag.message = "Berhasil menambah data";
            };

            return RedirectToAction("Index");
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
                attachments.CreatorUsername = this.User.Identity.Name;
                attachments.LastModifierUsername = this.User.Identity.Name;
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

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetByIdAdmin(id);

            return View(item);
            //return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductCatalogs model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> tvcvideo1, IEnumerable<IFormFile> tvcvideo2, IEnumerable<IFormFile> tvcvideo3)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model.Id);
                }

                model.LastModifierUsername = this.User.Identity.Name;
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

            }
            //RedirectToAction("Index");
            return Redirect("~/ProductCatalog/Index");
        }
        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = _appService.GetAllAdmin().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, SalesTalks item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Spesifikasi
        public IActionResult Spesifikasi(Guid id)
        {
            var item = _appService.GetByIdAdmin(id);

            return View(item);
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Spesifikasi(ProductCatalogs model, string submit)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);

                ViewBag.message = "Berhasil mengubah data";
            }
            return RedirectToAction("Index");
        }
        #endregion


        //public IActionResult GridAttachment_Read([DataSourceRequest]DataSourceRequest request, SalesTalks model)
        //{
        //    DataSourceResult result = _appService.GetAllAttachments(model.Id).Where(x=> string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x=>x.Title).ToDataSourceResult(request);
        //    return Json(result);
        //}

        //public IActionResult GridAttachment_Destroy([DataSourceRequest]DataSourceRequest request, SalesTalkAttachments item)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _attachmentAppService.SoftDelete(item.Id, this.User.Identity.Name);
        //    }

        //    return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        //}

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
        public IActionResult VarianWarna(Guid id)
        {
            var item = _appService.GetByIdAdmin(id);

            return View(item);
            //return View();
        }

        public IActionResult PartialVarianWarna(Guid id)
        {
            ProductColorVariants model = new ProductColorVariants();
            model.ProductCatalogId = id;
            return PartialView("PartialVariantWarna");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVarianWarna(ProductCatalogs model, string submit, IEnumerable<IFormFile> files, string picker)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return Redirect("~/ProductCatalog/VarianWarna/" + model.Id);
                    //return RedirectToAction("VarianWarna", model.Id);
                }
                ProductColorVariants colorVariants = new ProductColorVariants();
                colorVariants.Id = Guid.NewGuid();
                colorVariants.Title = model.Title;
                colorVariants.CreationTime = DateTime.Now;
                colorVariants.CreatorUsername = this.User.Identity.Name;
                colorVariants.LastModifierUsername = this.User.Identity.Name;
                colorVariants.LastModificationTime = DateTime.Now;
                colorVariants.DeleterUsername = "";
                colorVariants.ProductCatalogId = model.Id;
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

                TempData["alert"] = "";
                TempData["success"] = "Berhasil menambah data";
            }
            //model = _appService.GetById(model.Id);
            return Redirect("~/ProductCatalog/VarianWarna/" + model.Id);
        }

        public IActionResult EditPartialVarianWarna(Guid id)
        {
            ProductCatalogs model = new ProductCatalogs();
            ProductColorVariants colorVariants = _colorVariantsAppService.GetById(id);
            model.Title = colorVariants.Title;
            model.PanjangLebarTinggi = colorVariants.HexColor;
            model.FeaturedImageUrl = colorVariants.ImageUrl;
            return PartialView("EditPartialVariantWarna", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVarianWarna(ProductCatalogs model, string submit, IEnumerable<IFormFile> files, string picker)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return Redirect("~/ProductCatalog/VarianWarna/" + model.Id);
                    //return RedirectToAction("VarianWarna", model.Id);
                }
                ProductColorVariants colorVariants = _colorVariantsAppService.GetById(model.Id);

                colorVariants.Title = model.Title;
                colorVariants.HexColor = picker;
                colorVariants.LastModifierUsername = this.User.Identity.Name;
                colorVariants.LastModificationTime = DateTime.Now;

                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        colorVariants.ImageUrl = await InsertAndGetUrlAzure(file, colorVariants.Id.ToString(), "IMG", "productcatalogcolor");
                    }
                }

                _colorVariantsAppService.Update(colorVariants);

                TempData["alert"] = "";
                TempData["success"] = "Berhasil mengubah data";

                return Redirect("~/ProductCatalog/VarianWarna/" + colorVariants.ProductCatalogId);
            }
            //model = _appService.GetById(model.Id);
            return Redirect("~/ProductCatalog/VarianWarna/" + model.Id);
        }

        public IActionResult Grid_Read_VarianWarna([DataSourceRequest]DataSourceRequest request, ProductCatalogs model)
        {

            DataSourceResult result = _colorVariantsAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.ProductCatalogId == model.Id).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy_VarianWarna([DataSourceRequest]DataSourceRequest request, ProductColorVariants item)
        {
            if (ModelState.IsValid)
            {
                _colorVariantsAppService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Fitur Utama
        public IActionResult FiturUtama(Guid id)
        {
            var item = _appService.GetByIdAdmin(id);

            return View(item);
            //return View();
        }

        public IActionResult PartialFiturUtama(Guid id)
        {
            ProductFeatures model = new ProductFeatures();
            model.ProductCatalogId = id;
            return PartialView("PartialFiturUtama");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFiturUtama(ProductCatalogs model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return Redirect("~/ProductCatalog/FiturUtama/" + model.Id);
                    //return RedirectToAction("VarianWarna", model.Id);
                }

                ProductFeatures item = new ProductFeatures();
                item.Id = Guid.NewGuid();
                item.Title = model.Title;
                item.Description = model.TipeMesin;
                item.CreationTime = DateTime.Now;
                item.CreatorUsername = this.User.Identity.Name;
                item.LastModifierUsername = this.User.Identity.Name;
                item.LastModificationTime = DateTime.Now;
                item.DeleterUsername = "";
                item.ProductCatalogId = model.Id;
                item.ImageUrl = "";

                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        item.ImageUrl = await InsertAndGetUrlAzure(file, item.Id.ToString(), "IMG", "productcatalogmainfeature");
                    }
                }

                _featuresAppService.Create(item);

                TempData["alert"] = "";
                TempData["success"] = "Berhasil menambah data";
            }
            //model = _appService.GetById(model.Id);
            return Redirect("~/ProductCatalog/FiturUtama/" + model.Id);
        }

        public IActionResult EditPartialFiturUtama(Guid id)
        {
            ProductCatalogs model = new ProductCatalogs();
            ProductFeatures item = _featuresAppService.GetById(id);
            model.Title = item.Title;
            model.TipeMesin = item.Description;
            model.FeaturedImageUrl = item.ImageUrl;
            return PartialView("EditPartialFiturUtama", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFiturUtama(ProductCatalogs model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return Redirect("~/ProductCatalog/FiturUtama/" + model.Id);
                    //return RedirectToAction("VarianWarna", model.Id);
                }
                ProductFeatures item = _featuresAppService.GetById(model.Id);

                item.Title = model.Title;
                item.Description = model.TipeMesin;
                item.LastModifierUsername = this.User.Identity.Name;
                item.LastModificationTime = DateTime.Now;

                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        item.ImageUrl = await InsertAndGetUrlAzure(file, item.Id.ToString(), "IMG", "productcatalogmainfeature");
                    }
                }

                _featuresAppService.Update(item);

                TempData["alert"] = "";
                TempData["success"] = "Berhasil mengubah data";

                return Redirect("~/ProductCatalog/FiturUtama/" + item.ProductCatalogId);
            }
            //model = _appService.GetById(model.Id);
            return Redirect("~/ProductCatalog/FiturUtama/" + model.Id);
        }

        public IActionResult Grid_Read_FiturUtama([DataSourceRequest]DataSourceRequest request, ProductCatalogs model)
        {

            DataSourceResult result = _featuresAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.ProductCatalogId == model.Id).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy_FiturUtama([DataSourceRequest]DataSourceRequest request, ProductColorVariants item)
        {
            if (ModelState.IsValid)
            {
                _featuresAppService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Aksesoris
        public IActionResult Aksesoris(Guid id)
        {
            var item = _appService.GetByIdAdmin(id);

            return View(item);
            //return View();
        }

        public IActionResult PartialAksesoris(Guid id)
        {
            ProductAccesories model = new ProductAccesories();
            model.ProductCatalogId = id;
            return PartialView("PartialAksesoris");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAksesoris(ProductCatalogs model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return Redirect("~/ProductCatalog/Aksesoris/" + model.Id);
                    //return RedirectToAction("VarianWarna", model.Id);
                }
                if (model.PanjangLebarTinggi == null)
                {
                    TempData["alert"] = "Harga masih kosong atau belum diisi";
                    TempData["success"] = "";
                    return Redirect("~/ProductCatalog/Aksesoris/" + model.Id);
                    //return RedirectToAction("VarianWarna", model.Id);
                }
                ProductAccesories item = new ProductAccesories();
                item.Id = Guid.NewGuid();
                item.Title = model.Title;
                item.CreationTime = DateTime.Now;
                item.CreatorUsername = this.User.Identity.Name;
                item.LastModifierUsername = this.User.Identity.Name;
                item.LastModificationTime = DateTime.Now;
                item.DeleterUsername = "";
                item.ProductCatalogId = model.Id;
                item.ImageUrl = "";
                item.Price = int.Parse(model.PanjangLebarTinggi);
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

                TempData["alert"] = "";
                TempData["success"] = "Berhasil menambah data";
            }
            //model = _appService.GetById(model.Id);
            return Redirect("~/ProductCatalog/Aksesoris/" + model.Id);
        }

        public IActionResult EditPartialAksesoris(Guid id)
        {
            ProductCatalogs model = new ProductCatalogs();
            ProductAccesories item = _accesoriesAppService.GetById(id);
            model.Title = item.Title;
            model.PanjangLebarTinggi = item.Price.ToString();
            model.DiameterLangkah = item.Description;
            model.FeaturedImageUrl = item.ImageUrl;
            model.ProductCode = item.AccessoriesCode;
            return PartialView("EditPartialAksesoris", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAksesoris(ProductCatalogs model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return Redirect("~/ProductCatalog/Aksesoris/" + model.Id);
                    //return RedirectToAction("VarianWarna", model.Id);
                }
                if (model.PanjangLebarTinggi == null)
                {
                    TempData["alert"] = "Harga masih kosong atau belum diisi";
                    TempData["success"] = "";
                    return Redirect("~/ProductCatalog/Aksesoris/" + model.Id);
                    //return RedirectToAction("VarianWarna", model.Id);
                }
                ProductAccesories item = _accesoriesAppService.GetById(model.Id);

                item.Title = model.Title;
                item.Price = int.Parse(model.PanjangLebarTinggi);
                item.Description = model.DiameterLangkah;
                item.AccessoriesCode = model.ProductCode;
                item.LastModifierUsername = this.User.Identity.Name;
                item.LastModificationTime = DateTime.Now;

                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        item.ImageUrl = await InsertAndGetUrlAzure(file, item.Id.ToString(), "IMG", "productcatalogaccessories");
                    }
                }

                _accesoriesAppService.Update(item);

                TempData["alert"] = "";
                TempData["success"] = "Berhasil mengubah data";

                return Redirect("~/ProductCatalog/Aksesoris/" + item.ProductCatalogId);
            }
            //model = _appService.GetById(model.Id);
            return Redirect("~/ProductCatalog/Aksesoris/" + model.Id);
        }

        public IActionResult Grid_Read_Aksesoris([DataSourceRequest]DataSourceRequest request, ProductCatalogs model)
        {

            DataSourceResult result = _accesoriesAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.ProductCatalogId == model.Id).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy_Aksesoris([DataSourceRequest]DataSourceRequest request, ProductColorVariants item)
        {
            if (ModelState.IsValid)
            {
                _accesoriesAppService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Varian Harga

        public IActionResult VarianHarga(Guid id)
        {
            
            var item = _appService.GetByIdAdmin(id);

            return View(item);
            //return View();
        }

        public IActionResult PartialVarianHarga(Guid id)
        {
            ProductPrices model = new ProductPrices();

            return PartialView("PartialVariantHarga");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVarianHarga(ProductCatalogs model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var file = files.FirstOrDefault();
                    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) )
                    {
                        ViewBag.message = "Format file hanya mendukung .xlsx";
                        return Redirect("~/ProductCatalog/VarianHarga/" + model.Id);
                    }

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Template"];
                            var rowCount = worksheet.Dimension.Rows;

                            //TempData["alert"] = "Bahaya";
                            //return RedirectToAction("VarianHarga/" + model.Id);
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var kodeDealerMPM = worksheet.Cells[row, 1].Value.ToString().Trim();
                                var productColorVariantId = Guid.Parse(worksheet.Cells[row, 2].Value.ToString().Trim());
                                var priceStr = worksheet.Cells[row, 3].Value.ToString().Trim();
                                int price = 0;

                                if (_colorVariantsAppService.GetById(productColorVariantId) == null)
                                {
                                    TempData["alert"] = "Data yang dimasukkan dengan color variant id " + productColorVariantId + " tidak ditemukan";
                                    TempData["success"] = "";
                                    return RedirectToAction("VarianHarga/" + model.Id);
                                }
                                if (priceStr != "" || priceStr != null)
                                {
                                    price = int.Parse(priceStr);
                                }

                                if (_pricesAppService.GetAll().Where(x=>x.KodeDealerMPM == kodeDealerMPM && x.ProductColorVariantId == productColorVariantId).Count() > 0)
                                {
                                    var attachment = _pricesAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == kodeDealerMPM && x.ProductColorVariantId == productColorVariantId);
                                    attachment.Price = price;
                                    _pricesAppService.Update(attachment);
                                    //TempData["alert"] = "Data dengan kode dealer " + attachment.KodeDealerMPM + " dan color variant id " + attachment.ProductColorVariantId;
                                    //TempData["success"] = "";
                                    //return RedirectToAction("VarianHarga/" + model.Id);
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
                                        CreatorUsername = this.User.Identity.Name,
                                        LastModifierUsername = this.User.Identity.Name,
                                        LastModificationTime = DateTime.Now,
                                        DeleterUsername = ""
                                    };
                                    _pricesAppService.Create(attachment);
                                }
                                
                            }
                        }
                    }
                }

                TempData["success"] =  "Berhasil mengubah data";
                TempData["alert"] = "";
            }
            model = _appService.GetByIdAdmin(model.Id);
            return Redirect("~/ProductCatalog/VarianHarga/" + model.Id);
            //return Redirect("~/ProductCatalog");
        }

        public ActionResult DownloadTemplate(ProductCatalogs model)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Template Variant Harga.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Template");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Kode Dealer";
                    workSheet.Cells[1, 2].Value = "Product Color Variant Id";
                    workSheet.Cells[1, 3].Value = "Harga";

                    workSheet.Cells[2, 1].Value = "A0010";
                    workSheet.Cells[2, 2].Value = "32d7c6dc-d9d7-48da-b6c5-476891228351";
                    workSheet.Cells[2, 3].Value = "1000000";

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();

                    var workSheetColor = package.Workbook.Worksheets.Add("Colors");

                    workSheetColor.Cells[1, 1].Value = "Product Color Name";
                    workSheetColor.Cells[1, 2].Value = "Product Color Variant Id";

                    var colorVariants = _appService.GetByIdAdmin(model.Id).ProductColorVariants;
                    int i = 2;
                    foreach (var colorVariant in colorVariants)
                    {
                        workSheet.Cells[i, 10].Value = colorVariant.Title;
                        workSheet.Cells[i, 11].Value = colorVariant.Id;
                        i++;
                    }

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();

                    package.Save();
                }
            }
            catch(Exception ex)
            {

            }
            
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        public IActionResult Grid_Read_VarianHarga([DataSourceRequest]DataSourceRequest request, ProductCatalogs model)
        {
            var colorVariantId = _pricesAppService.GetAll().ToList(); 
            DataSourceResult result = _pricesAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

            return Json(result);

        }
        #endregion
    }
}