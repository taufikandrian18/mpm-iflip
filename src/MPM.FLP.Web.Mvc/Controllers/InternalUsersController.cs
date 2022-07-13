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
using MPM.FLP.Authorization.Users;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.DataValidation;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class InternalUsersController : FLPControllerBase
    {
        private readonly InternalUserAppService _appService;
        private readonly UserManager _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public InternalUsersController(InternalUserAppService appService, UserManager userManager, IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit(string id)
        {
            int idMpm = int.Parse(id);
            var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
            var roles = _userManager.GetRolesAsync(user).Result.ToList();
            string channel = "";
            if (roles.FirstOrDefault().Contains("H1"))
            {
                channel = "H1";
            }
            else if (roles.FirstOrDefault().Contains("H2"))
            {
                channel = "H2";
            }
            var item = Task.Run(() => _appService.GetAllInternalUsers(channel)).Result.SingleOrDefault(x => x.IDMPM == idMpm);

            return View(item);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(InternalUserDto model)
        {
            if (model != null)
            {
                
                UpdateInternalUserDto item = new UpdateInternalUserDto {
                    AbpUserId = (int)model.AbpUserId,
                    IsActive = model.IsActive,
                    LastModifierUser = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId()),
                    LastModificationTime = DateTime.Now
                };

                await _appService.Update(item);
            }
            return Json(new { success = true });
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
            var roles = _userManager.GetRolesAsync(user).Result.ToList();
            string channel = "";
            if (roles.FirstOrDefault().Contains("H1"))
            {
                channel = "H1";
            }
            else if(roles.FirstOrDefault().Contains("H2"))
            {
                channel = "H2";
            }

            var task = Task.Run(() => _appService.GetAllInternalUsers(channel));
            
            DataSourceResult result = task.Result?.ToDataSourceResult(request);

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
            string excelName = "Internal Users.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Internal Users");
                    var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
                    var roles = _userManager.GetRolesAsync(user).Result.ToList();
                    string channel = "";

                    if (roles.FirstOrDefault().Contains("H1"))
                    {
                        channel = "H1";
                    }
                    else if (roles.FirstOrDefault().Contains("H2"))
                    {
                        channel = "H2";
                    }

                    var task = Task.Run(() => _appService.GetAllInternalUsers(channel));

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Id MPM";
                    workSheet.Cells[1, 2].Value = "Id Honda";
                    workSheet.Cells[1, 3].Value = "Nama";
                    workSheet.Cells[1, 4].Value = "Gender";
                    workSheet.Cells[1, 5].Value = "Alamat";
                    workSheet.Cells[1, 6].Value = "Channel";
                    workSheet.Cells[1, 7].Value = "Handphone";
                    workSheet.Cells[1, 8].Value = "Jabatan";
                    workSheet.Cells[1, 9].Value = "Nama Dealer";
                    workSheet.Cells[1, 10].Value = "Kota Dealer";

                    int row = 2;
                    foreach(var result in task.Result)
                    {
                        workSheet.Cells[row, 1].Value = result.IDMPM;
                        workSheet.Cells[row, 2].Value = result.IDHonda;
                        workSheet.Cells[row, 3].Value = result.Nama;
                        workSheet.Cells[row, 4].Value = result.Gender;
                        workSheet.Cells[row, 5].Value = result.Alamat;
                        workSheet.Cells[row, 6].Value = result.Channel;
                        workSheet.Cells[row, 7].Value = result.Handphone;
                        workSheet.Cells[row, 8].Value = result.Jabatan;
                        workSheet.Cells[row, 9].Value = result.DealerName;
                        workSheet.Cells[row, 10].Value = result.DealerKota;

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
                    workSheet.Column(10).AutoFit();

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