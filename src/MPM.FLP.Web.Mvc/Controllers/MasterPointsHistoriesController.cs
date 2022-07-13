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
using MPM.FLP.Web.Models.FLPMPM;
using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.DataValidation;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Globalization;

namespace MPM.FLP.Web.Mvc.Controllers
{
    public class MasterPointsHistoriesController : FLPControllerBase
    {
        private readonly SalesPeopleDevelopmentContestAppService _appService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly InternalUserAppService _internalUserAppService;

        public MasterPointsHistoriesController(SalesPeopleDevelopmentContestAppService pointsAppService, IHostingEnvironment hostingEnvironment, InternalUserAppService internalUserAppService)
        {
            _appService = pointsAppService;
            _hostingEnvironment = hostingEnvironment;
            _internalUserAppService = internalUserAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            var result = _appService.GetAllPointHistory().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime);
            List<PointHistoriesVM> listPointHistoriesVMs = new List<PointHistoriesVM>();
            foreach(var item in result)
            {
                listPointHistoriesVMs.Add(new PointHistoriesVM
                {
                    Id = item.Id,
                    Nama = item.InternalUsers.Nama,
                    Channel = item.InternalUsers.Channel,
                    Dealer = item.InternalUsers.DealerName,
                    MasterPoint = item.SPDCMasterPoints.Title,
                    Point = item.Point,
                    Periode = item.Periode
                });
            }


            return Json(listPointHistoriesVMs.ToDataSourceResult(request));
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, SPDCPointHistories item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDeletePointHistory(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult Create()
        {
            SPDCPointHistories model = new SPDCPointHistories();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SPDCPointHistories model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
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
                            var categories = _appService.GetAllMasterPoint().Where(x => string.IsNullOrEmpty(x.DeleterUsername));

                            for (int row = 2; row <= rowCount; row++)
                            {
                                int i = 3;
                                foreach(var category in categories)
                                {
                                    int idmpm = int.Parse(worksheet.Cells[row, 1].Value.ToString());
                                    if (!_internalUserAppService.GetAll().Any(x => x.IDMPM == idmpm))
                                    {
                                        TempData["alert"] = "Id MPM " + idmpm + " pada baris " + row + " tidak ditemukan dalam database";
                                        TempData["success"] = "";
                                        return RedirectToAction("Create");
                                    }
                                    var period = worksheet.Cells[row, 2].Value.ToString();
                                    DateTime dateTime;
                                    DateTime periode = new DateTime();
                                    if (DateTime.TryParseExact(period, "dd/MM/yyyy", new CultureInfo("id-ID"), DateTimeStyles.None, out dateTime))
                                    {
                                        periode = DateTime.ParseExact(period, "dd/MM/yyyy", null);
                                    }
                                    else
                                    {
                                        long dateNum = long.Parse(period);
                                        periode = DateTime.FromOADate(dateNum);
                                    }
                                        
                                    //var masterPoint = worksheet.Cells[row, 3].Value.ToString();
                                    var masterPoint = worksheet.Cells[1, i].Value.ToString();
                                    var mpId = _appService.GetAllMasterPoint().Where(x => x.Title == masterPoint && string.IsNullOrEmpty(x.DeleterUsername)).Select(x => x.Id).SingleOrDefault();
                                    //var point = int.Parse(worksheet.Cells[row, 4].Value.ToString());
                                    var point = int.Parse(worksheet.Cells[row, i].Value.ToString());
                                    SPDCPointHistories clubCommunities = new SPDCPointHistories
                                    {
                                        Id = Guid.NewGuid(),
                                        CreationTime = DateTime.Now,
                                        CreatorUsername = this.User.Identity.Name,
                                        LastModifierUsername = this.User.Identity.Name,
                                        LastModificationTime = DateTime.Now,
                                        DeleterUsername = "",
                                        IDMPM = idmpm,
                                        SPDCMasterPointId = mpId,
                                        Point = point,
                                        Periode = periode
                                    };
                                    _appService.CreatePointHisotry(clubCommunities);
                                    i++;
                                }
                            }
                        }
                    }
                }
                else
                {
                    TempData["success"] = "";
                    TempData["alert"] = "File belum dipilih atau format file salah";
                    return RedirectToAction("Create");
                }

                TempData["success"] = "Berhasil menambah data";
                TempData["alert"] = "";
            }
            return Redirect("Index");

        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetAllPointHistory().Where(x=>x.Id == id).SingleOrDefault();
            
            return View(item);

            //return View();
        }

        [HttpPost]
        public IActionResult Edit(SPDCPointHistories model, string submit)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;
                model.SPDCMasterPoints = null;

                _appService.UpdatePointHistory(model);

                return Json(new { success = true });
            }

            return View(model);
        }

        public ActionResult DownloadTemplate(ProductCatalogs model)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Template Point Histories.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Template");
                    var workSheetCategories = package.Workbook.Worksheets.Add("Master Point");
                    var categories = _appService.GetAllMasterPoint().Where(x => string.IsNullOrEmpty(x.DeleterUsername));

                    //var validation = workSheet.DataValidations.AddListValidation("C2:C1000");
                    //validation.ShowErrorMessage = true;
                    //validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    //validation.ErrorTitle = "An invalid value was entered";
                    //validation.Error = "Select a value from the list";

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheetCategories.Cells[1, 1].Value = "Nama Master Point";
                    workSheetCategories.Cells[1, 2].Value = "Bobot Point";

                    //set worksheet categories
                    int i = 2;
                    foreach (var category in categories)
                    {
                        workSheetCategories.Cells[i, 1].Value = category.Title;
                        workSheetCategories.Cells[i, 2].Value = category.Weight;
                        //validation.Formula.Values.Add(category.Title);
                        i++;
                    }
                    i = 3;
                    foreach(var category in categories)
                    {
                        workSheet.Cells[1, i].Value = category.Title;
                        workSheet.Cells[2, i].Value = 100;
                        workSheet.Column(i).AutoFit();
                        i++;
                    }

                    //set header template
                    workSheet.Cells[1, 1].Value = "Id MPM";
                    workSheet.Cells[2, 1].Value = "17";
                    workSheet.Cells[1, 2].Value = "Periode";
                    workSheet.Cells[2, 2].Value = DateTime.Now.ToString("dd/MM/yyyy");
                    //workSheet.Cells[1, 3].Value = "Master Point";
                    //workSheet.Cells[1, 4].Value = "Bobot Point";
                    //workSheet.Cells[2, 4].Value = 100;

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(1).Style.Numberformat.Format = "0";
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(2).Style.Numberformat.Format = "dd/MM/yyyy";
                    //workSheet.Column(3).AutoFit();
                    //workSheet.Column(4).AutoFit();

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