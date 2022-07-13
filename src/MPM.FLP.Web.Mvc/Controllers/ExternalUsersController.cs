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
using Abp.Runtime.Security;
using MPM.FLP.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using MPM.FLP.Services.Dto;
using MPM.FLP.Authorization.Users;

namespace MPM.FLP.Web.Mvc.Controllers
{
    public class ExternalUsersController : FLPControllerBase
    {
        private readonly  ExternalUserAppService _appService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager _userManager;

        public ExternalUsersController(ExternalUserAppService appService, IHostingEnvironment hostingEnvironment, UserManager userManager)
        {
            _appService = appService;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit(string id)
        {
            long? abpuser = long.Parse(id);
            var item = Task.Run(() => _appService.GetAll()).Result.SingleOrDefault(x=>x.AbpUserId == abpuser);

            return View(item);
        }


        [HttpPost]
        public async Task<IActionResult> EditExternal(ExternalUserDto model)
        {
            if (model != null)
            {
                UpdateExternalUserDto item = new UpdateExternalUserDto();
                item.Id = Task.Run(() => _appService.GetAll()).Result.SingleOrDefault(x => x.AbpUserId == model.AbpUserId).Id;
                item.AbpUserId = (int)model.AbpUserId;
                item.IsKTPVerified = model.IsKTPVerified;
                item.IsActive = model.IsActive;
                item.LastModifierUser = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
                item.LastModificationTime = DateTime.UtcNow.AddHours(7);
                await _appService.Update(item);

                // _appService.Update(model);
            }
            return Json(new { success = true });
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            var task = Task.Run(() => _appService.GetAll());
            
            DataSourceResult result = task.Result.ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, SalesTalks item)
        {
            if (ModelState.IsValid)
            {
                //_appService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ExportExcel()
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "External Users.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("External Users");
                    

                    var task = Task.Run(() => _appService.GetAll());

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Nama";
                    workSheet.Cells[1, 2].Value = "Nama Toko";
                    workSheet.Cells[1, 3].Value = "Alamat";
                    workSheet.Cells[1, 4].Value = "Longitude";
                    workSheet.Cells[1, 5].Value = "Latitude";
                    workSheet.Cells[1, 6].Value = "Channel";
                    workSheet.Cells[1, 7].Value = "Handphone";
                    workSheet.Cells[1, 8].Value = "Jabatan";
                    workSheet.Cells[1, 9].Value = "Email";

                    int row = 2;
                    foreach (var result in task.Result)
                    {
                        workSheet.Cells[row, 1].Value = result.Name;
                        workSheet.Cells[row, 2].Value = result.ShopName;
                        workSheet.Cells[row, 3].Value = result.Address;
                        workSheet.Cells[row, 4].Value = result.Longitude;
                        workSheet.Cells[row, 5].Value = result.Latitude;
                        workSheet.Cells[row, 6].Value = result.Channel;
                        workSheet.Cells[row, 7].Value = result.Handphone;
                        workSheet.Cells[row, 8].Value = result.Jabatan;
                        workSheet.Cells[row, 9].Value = result.Email;

                        row++;
                    }

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();
                    workSheet.Column(6).AutoFit();
                    workSheet.Column(7).AutoFit();
                    workSheet.Column(8).AutoFit();
                    workSheet.Column(9).AutoFit();

                    //var validation = workSheet.DataValidations.AddListValidation("G2:G1000");
                    //validation.ShowErrorMessage = true;
                    //validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    //validation.ErrorTitle = "An invalid value was entered";
                    //validation.Error = "Select a value from the list";
                    //validation.Formula.Values.AddRange(new List<string> { "A", "B", "C", "D", "E" });
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