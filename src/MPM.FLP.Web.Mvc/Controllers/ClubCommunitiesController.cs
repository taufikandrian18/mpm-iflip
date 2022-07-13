using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using MPM.FLP.Controllers;
using MPM.FLP.Services;
using System.Collections;
using System.Collections.Generic;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MPM.FLP.EntityFrameworkCore;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.DataValidation;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class ClubCommunitiesController : FLPControllerBase
    {
        private readonly ClubCommunityAppService _appService;
        private readonly ClubCommunityCategoryAppService _categoriesAppService;

        private readonly IHostingEnvironment _hostingEnvironment;

        public ClubCommunitiesController(ClubCommunityAppService appService, ClubCommunityCategoryAppService categoriesAppService, IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _categoriesAppService = categoriesAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            return View();
        }

        public IActionResult GetCategorys([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = _categoriesAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Create(ClubCommunities model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClubCommunities model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var file = files.FirstOrDefault();
                    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewBag.message = "Format file hanya mendukung .xlsx";
                        return Redirect("~/ClubCommunities/Create" + model.Id);
                    }

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Template Club Communities"];

                            if(worksheet == null)
                            {
                                TempData["alert"] = "Template yang digunakan salah";
                                TempData["success"] = "";
                                return RedirectToAction("Create/" + model.Id);
                            }

                            var rowCount = worksheet.Dimension.Rows;

                            //TempData["alert"] = "Bahaya";
                            //return RedirectToAction("VarianHarga/" + model.Id);
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var clubName = worksheet.Cells[row, 1].Value?.ToString().Trim();
                                var categoriesClub = worksheet.Cells[row, 2].Value?.ToString().Trim();
                                //var categoriesClub = Guid.Parse(worksheet.Cells[row, 2].Value.ToString().Trim());
                                var contactPerson = worksheet.Cells[row, 3].Value?.ToString().Trim();
                                var contactNumber = worksheet.Cells[row, 4].Value?.ToString().Trim();
                                var email = worksheet.Cells[row, 5].Value?.ToString().Trim();
                                var city = worksheet.Cells[row, 6].Value?.ToString().Trim();
                                //var city = worksheet.Cells[row, 7].Value.ToString().Trim();

                                //if (_categoriesAppService.GetById(categoriesClub) == null)
                                //{
                                //    TempData["alert"] = "Data yang dimasukkan dengan Kategori Klub Id " + categoriesClub + " pada baris " + row +" tidak ditemukan";
                                //    TempData["success"] = "";
                                //    return RedirectToAction("Create/" + model.Id);
                                //}
                                if (clubName == null)
                                {
                                    TempData["alert"] = "Data nama klub pada baris " + row + " masih kosong";
                                    TempData["success"] = "";
                                    return RedirectToAction("Create/" + model.Id);
                                }
                                if (categoriesClub == null)
                                {
                                    TempData["alert"] = "Data nama kategori klub pada baris " + row + " masih kosong";
                                    TempData["success"] = "";
                                    return RedirectToAction("Create/" + model.Id);
                                }
                                if (contactPerson == null)
                                {
                                    TempData["alert"] = "Data nama kontak pada baris " + row + " masih kosong";
                                    TempData["success"] = "";
                                    return RedirectToAction("Create/" + model.Id);
                                }
                                if (contactNumber == null)
                                {
                                    TempData["alert"] = "Data nomor kontak pada baris " + row + " masih kosong";
                                    TempData["success"] = "";
                                    return RedirectToAction("Create/" + model.Id);
                                }
                                if (email == null)
                                {
                                    TempData["alert"] = "Data email pada baris " + row + " masih kosong";
                                    TempData["success"] = "";
                                    return RedirectToAction("Create/" + model.Id);
                                }
                                if (city == null)
                                {
                                    TempData["alert"] = "Data kota pada baris " + row + " masih kosong";
                                    TempData["success"] = "";
                                    return RedirectToAction("Create/" + model.Id);
                                }
                                var categoriesClubId = Guid.NewGuid();
                                try
                                {
                                    categoriesClubId = _categoriesAppService.GetAll().FirstOrDefault(x => x.Name == categoriesClub && string.IsNullOrEmpty(x.DeleterUsername)).Id;
                                }
                                catch(Exception ex)
                                {
                                    var c = ex;
                                }
                                ClubCommunities clubCommunities = new ClubCommunities
                                {
                                    Id = Guid.NewGuid(),
                                    Name = clubName,
                                    ClubCommunityCategoryId = categoriesClubId,
                                    ContactPerson = contactPerson,
                                    ContactNumber = contactNumber,
                                    Email = email,
                                    Kota = city,
                                    CreationTime = DateTime.Now,
                                    CreatorUsername = this.User.Identity.Name,
                                    LastModifierUsername = this.User.Identity.Name,
                                    LastModificationTime = DateTime.Now,
                                    DeleterUsername = ""
                                };
                                _appService.Create(clubCommunities);
                            }
                        }
                    }
                }

                TempData["success"] = "Berhasil menambah data";
                TempData["alert"] = "";
            }
            return Redirect("Index");
        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetById(id);

            return View(item);
            //return View();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ClubCommunities model, string submit)
        {
            if (model != null)
            {
                if (model.Name == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model.Id);
                }
                if (model.ContactPerson == null)
                {
                    TempData["alert"] = "Nama kontak person masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }
                if (model.ClubCommunityCategoryId == null)
                {
                    TempData["alert"] = "Kategori belum dipilih";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {

            DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

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

        public ActionResult DownloadTemplate(ProductCatalogs model)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Template Club Communities.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Template Club Communities");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Nama Klub";
                    workSheet.Cells[1, 2].Value = "Kategori Klub";
                    workSheet.Cells[1, 3].Value = "Nama Kontak";
                    workSheet.Cells[1, 4].Value = "Nomor Kontak";
                    workSheet.Cells[1, 5].Value = "E-mail";
                    workSheet.Cells[1, 6].Value = "Kota";

                    workSheet.Cells[2, 1].Value = "Honda Club";
                    //workSheet.Cells[2, 2].Value = "fed5b102-ddbf-46bd-9936-18f07a4f697d";
                    workSheet.Cells[2, 3].Value = "MPM";
                    workSheet.Cells[2, 4].Value = "088812345678";
                    workSheet.Cells[2, 5].Value = "flpmpm@email.com";
                    workSheet.Cells[2, 6].Value = "Surabaya";

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();
                    workSheet.Column(6).AutoFit();

                    var validation = workSheet.DataValidations.AddListValidation("B2:B200");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    validation.ErrorTitle = "An invalid value was entered";
                    validation.Error = "Select a value from the list";

                    var workSheetCategories = package.Workbook.Worksheets.Add("Kategori Klub");
                    workSheetCategories.Cells[1, 1].Value = "Kategori Klub";
                    workSheetCategories.Cells[1, 2].Value = "Kategori Klub Id";

                    var categories = _categoriesAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
                    int i = 2;
                    foreach (var category in categories)
                    {
                        workSheetCategories.Cells[i, 1].Value = category.Name;
                        workSheetCategories.Cells[i, 2].Value = category.Id;
                        validation.Formula.Values.Add(category.Name);
                        i++;
                    }

                    workSheetCategories.Column(1).AutoFit();
                    workSheetCategories.Column(2).AutoFit();
                    package.Save();
                }
            }
            catch (Exception ex)
            {

            }

            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }


    }
}