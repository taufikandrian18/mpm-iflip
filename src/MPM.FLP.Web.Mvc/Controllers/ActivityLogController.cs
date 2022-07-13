using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using MPM.FLP.Controllers;
using MPM.FLP.Services;
using System.Collections.Generic;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Authorization.Roles;
using MPM.FLP.Services.Dto;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Abp.Web.Models;
using Abp.Runtime.Validation;
using MPM.FLP.Web.Models.FLPMPM;
using MPM.FLP.FLPDb;
using Microsoft.EntityFrameworkCore;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class ActivityLogController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly InternalUserAppService _internalUserAppService;
        private readonly IActivityLogAppService _activityLogAppService;
        private readonly IArticleAppService _appService;
        private readonly IArticleAttachmentAppService _attachmentAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ActivityLogController(IActivityLogAppService activityLogAppService, IArticleAppService appService, IArticleAttachmentAppService attachmentAppService, UserManager userManager,  RoleManager roleManager, IHostingEnvironment hostingEnvironment, InternalUserAppService internalUserAppService)
        {
            _activityLogAppService = activityLogAppService;
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _internalUserAppService = internalUserAppService;
        }

        public IActionResult BackToIndex()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            return View();
        }

        public async Task<IActionResult> Search(int year, int month, string selection)
        {
            if(selection == "Activity")
            {
                List<GetActivityLogSummaryDto> result = new List<GetActivityLogSummaryDto>();
                if (year != 0 && month == 0)
                {
                    result = await _activityLogAppService.GetActivityLogsSummaryByYear(year);
                }
                else
                {
                    result = await _activityLogAppService.GetActivityLogsSummaryByMonth(year,month);
                }

                if (result.Count > 0)
                    return Json(new { success = true, item = result });
                else
                    return Json(new { success = false });
            }
            else
            {
                List<GetContentActivityLogSummaryDto> result;
                if (year != 0 && month == 0)
                {
                    result = await _activityLogAppService.GetContentActivityLogsSummary(year, null);
                }
                else
                {
                    result = await _activityLogAppService.GetContentActivityLogsSummary(year, month);
                }

                if (result.Count > 0)
                    return Json(new { success = true, item = result });
                else
                    return Json(new { success = false });
            }
        }

        #region Detail Internal

        public IActionResult DetailGate(string contentId, string year, string month = "")
        {
            var tmp = _activityLogAppService.GetAll().Result.ToList().Where(x => x.ContentId == contentId && x.Time.Year == int.Parse(year));
            if (int.Parse(month) != 0) tmp = tmp.Where(x => x.Time.Month == int.Parse(month));


            return View("DetailInternal");
        }

        public IActionResult DetailInternal(string contentId)
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            return View();
        }
        [DontWrapResult(WrapOnError = false, WrapOnSuccess = false)]
        public async Task<IActionResult> Grid_Internal_Detail([DataSourceRequest]DataSourceRequest request, GetContentActivityLogSummaryDto model)
        {
            var tmp = await _activityLogAppService.GetAll();
            List<ActivityDetailInternalVM> list = new List<ActivityDetailInternalVM>();
            string serializer = JsonConvert.SerializeObject(list.ToDataSourceResult(request));

            return Content(serializer, "application/json");
        }
        #endregion

        [DisableValidation]
        public async Task<IActionResult> DownloadExcel(int year, int month, string selection)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Activity Log.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Log");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Content Type";
                    
                    

                    if (selection == "Activity")
                    {
                        List<GetActivityLogSummaryDto> result = new List<GetActivityLogSummaryDto>();
                        if (year != 0 && month == 0)
                        {
                            result = await _activityLogAppService.GetActivityLogsSummaryByYear(year);
                        }
                        else
                        {
                            result = await _activityLogAppService.GetActivityLogsSummaryByMonth(year, month);
                        }

                        workSheet.Cells[1, 2].Value = "Activity Type";
                        workSheet.Cells[1, 3].Value = "Count";

                        int row = 2;
                        foreach (var item in result)
                        {
                            workSheet.Cells[row, 1].Value = item.ContentType;
                            workSheet.Cells[row, 2].Value = item.ActivityType;
                            workSheet.Cells[row, 3].Value = item.Count;

                            row++;
                        }
                    }
                    else
                    {
                        List<GetContentActivityLogSummaryDto> result = new List<GetContentActivityLogSummaryDto>();
                        if (year != 0 && month == 0)
                        {
                            result = await _activityLogAppService.GetContentActivityLogsSummary(year, null);
                        }
                        else
                        {
                            result = await _activityLogAppService.GetContentActivityLogsSummary(year, month);
                        }

                        workSheet.Cells[1, 2].Value = "Activity Type";
                        workSheet.Cells[1, 3].Value = "Content Id";
                        workSheet.Cells[1, 4].Value = "Content Title";
                        workSheet.Cells[1, 5].Value = "Count";

                        int row = 2;
                        foreach (var item in result)
                        {
                            workSheet.Cells[row, 1].Value = item.ContentType;
                            workSheet.Cells[row, 2].Value = item.ActivityType;
                            workSheet.Cells[row, 3].Value = item.ContentId;
                            workSheet.Cells[row, 4].Value = item.ContentTitle;
                            workSheet.Cells[row, 5].Value = item.Count;

                            row++;
                        }
                    }

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();

                    package.Save();
                }
            }
            catch (Exception ex)
            {

            }

            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [DisableValidation]
        public async Task<IActionResult> DownloadAll(int year, int month, string channel)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Activity Log Person "+ year.ToString() + "-" + month.ToString() +".xlsx";

            var stream = new MemoryStream();
            //List<string> intUsernames = _activityLogAppService.GetAll().Result.Where(x => int.TryParse(x.Username, out int i) == true && x.Time.Year == year).Where(x => x.Time.Month == month).Select(x => x.Username).Distinct().ToList();



            //var internalUsers = await _internalUserAppService.GetInternalUserByChannelIncludeActivity(channel);


            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    #region All Internal User

                    //for(int j = 0; j <= 2; j++) { 
                    //foreach (var internalUser in internalUsers)
                    //{
                        //if(internalUser != null)
                        {
                            var workSheet = package.Workbook.Worksheets.Add("Activities");
                            workSheet.Row(1).Height = 20;
                            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Row(1).Style.Font.Bold = true;

                            //workSheet.Cells[1, 6].Value = "Nama";
                            //workSheet.Cells[1, 7].Value = "Kode Dealer";
                            //workSheet.Cells[1, 8].Value = "Nama Dealer";
                            //workSheet.Cells[1, 9].Value = "Kota";

                            //workSheet.Cells[2, 6].Value = internalUser.Name;
                            //workSheet.Cells[2, 7].Value = internalUser.KodeDealer;
                            //workSheet.Cells[2, 8].Value = internalUser.NamaDealer;
                            //workSheet.Cells[2, 9].Value = internalUser.Kota;

                            workSheet.Cells[1, 1].Value = "IDMPM";
                            workSheet.Cells[1, 2].Value = "Content Type";
                            workSheet.Cells[1, 3].Value = "Activity Type";
                            workSheet.Cells[1, 4].Value = "Content Title";
                            workSheet.Cells[1, 5].Value = "Reading Time";

                        var activities = await _activityLogAppService.GetAllSpecified( year, month);

                        int i = 2;
                            foreach (var activity in activities)
                            {
                                workSheet.Cells[i, 1].Value = activity.Username;
                                workSheet.Cells[i, 2].Value = activity.ContentType;
                                workSheet.Cells[i, 3].Value = activity.ActivityType;
                                workSheet.Cells[i, 4].Value = activity.ContentTitle;
                                workSheet.Cells[i, 5].Style.Numberformat.Format = "dd-m-yyyy hh:mm";
                                //workSheet.Cells[i, 4].Formula = "=DATE(2014,10,5)";
                                workSheet.Cells[i, 5].Value = activity.Time;

                                i++;
                            }

                            workSheet.Column(1).AutoFit();
                            workSheet.Column(2).AutoFit();
                            workSheet.Column(3).AutoFit();
                            workSheet.Column(4).AutoFit();
                            workSheet.Column(6).AutoFit();
                            workSheet.Column(7).AutoFit();
                            workSheet.Column(8).AutoFit();
                            workSheet.Column(9).AutoFit();
                        }
                    //}
                    #endregion

                     package.Save();
                }
            }
            catch (Exception ex)
            {

            }

            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [DisableValidation]
        public async Task<IActionResult> DownloadRegisteredUser(int year, int month, string channel)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            //string excelName = "Activity Log Registered User " + year.ToString() + "-" + month.ToString() + ".xlsx";
            string excelName = "Activity Log Registered User.csv";

            var stream = new MemoryStream();
            //List<string> intUsernames = _activityLogAppService.GetAll().Result.Where(x => int.TryParse(x.Username, out int i) == true && x.Time.Year == year).Where(x => x.Time.Month == month).Select(x => x.Username).Distinct().ToList();
            //List<string> extUsernames = _activityLogAppService.GetAll().Result.Where(x => int.TryParse(x.Username, out int i) == false && x.Time.Year == year).Where(x => x.Time.Month == month).Select(x => x.Username).Distinct().ToList();

            //List<InternalUsers> internalUsers = new List<InternalUsers>();
            //foreach (var item in intUsernames)
            //{
            //    var internalUser = _internalUserAppService.GetInternalUserByChannel(channel).SingleOrDefault(x => x.IDMPM == int.Parse(item));
            //    internalUsers.Add(internalUser);
            //}

            var query = _internalUserAppService.GetAll().Where(x => x.AbpUserId != null).AsQueryable();
            if (year != 0)
            {
                query = query.Where(x => x.CreationTime.Year == year);
            }

            if (month != 0) {
                query = query.Where(x => x.CreationTime.Month == month);
            }

            var internalUsers = query.OrderBy(x => x.IDMPM);

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    #region All Internal User
                    var workSheet = package.Workbook.Worksheets.Add("Registered User");
                    //for(int j = 0; j <= 2; j++) { 
                    int row = 2;
                    foreach (var internalUser in internalUsers)
                    {
                        if (internalUser != null)
                        {
                            
                            workSheet.Row(1).Height = 20;
                            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Row(1).Style.Font.Bold = true;

                            workSheet.Cells[1, 1].Value = "Id MPM";
                            workSheet.Cells[1, 2].Value = "Nama";
                            workSheet.Cells[1, 3].Value = "Id Jabatan";
                            workSheet.Cells[1, 4].Value = "Jabatan";
                            workSheet.Cells[1, 5].Value = "Kode Dealer";
                            workSheet.Cells[1, 6].Value = "Nama Dealer";
                            workSheet.Cells[1, 7].Value = "Kota";
                            workSheet.Cells[1, 8].Value = "No. Hp";

                            workSheet.Cells[row, 1].Value = internalUser.IDMPM;
                            workSheet.Cells[row, 2].Value = internalUser.Nama;
                            workSheet.Cells[row, 3].Value = internalUser.IDJabatan;
                            workSheet.Cells[row, 4].Value = internalUser.Jabatan;
                            workSheet.Cells[row, 5].Value = internalUser.KodeDealerMPM;
                            workSheet.Cells[row, 6].Value = internalUser.DealerName;
                            workSheet.Cells[row, 7].Value = internalUser.DealerKota;
                            workSheet.Cells[row, 8].Value = internalUser.Handphone;

                            workSheet.Column(1).AutoFit();
                            workSheet.Column(2).AutoFit();
                            workSheet.Column(3).AutoFit();
                            workSheet.Column(4).AutoFit();
                            workSheet.Column(6).AutoFit();
                            workSheet.Column(7).AutoFit();
                            workSheet.Column(8).AutoFit();
                            workSheet.Column(9).AutoFit();
                        }

                        row++;
                    }
                    #endregion

                    package.Save();
                }
            }
            catch (Exception ex)
            {

            }

            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        private async Task<ExcelPackage> InsertInternalExcelValue(InternalUsers internalUser, int year, int month, ExcelPackage package)
        {
            var workSheet = package.Workbook.Worksheets.Add(internalUser.IDMPM.ToString());

            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells[1, 6].Value = "Nama";
            workSheet.Cells[1, 7].Value = "Kode Dealer";
            workSheet.Cells[1, 8].Value = "Nama Dealer";
            workSheet.Cells[1, 9].Value = "Kota";

            workSheet.Cells[2, 6].Value = internalUser.Nama;
            workSheet.Cells[2, 7].Value = internalUser.KodeDealerMPM;
            workSheet.Cells[2, 8].Value = internalUser.DealerName;
            workSheet.Cells[2, 9].Value = internalUser.DealerKota;

            workSheet.Cells[1, 1].Value = "Content Type";
            workSheet.Cells[1, 2].Value = "Activity Type";
            workSheet.Cells[1, 3].Value = "Content Title";
            workSheet.Cells[1, 4].Value = "Reading Time";

            var activities = _activityLogAppService.GetAll().Result.Where(x => x.Username == internalUser.IDMPM.ToString() && x.Time.Year == year).Where(x => x.Time.Month == month);

            int i = 2;
            foreach (var activity in activities)
            {
                workSheet.Cells[i, 1].Value = activity.ContentType;
                workSheet.Cells[i, 2].Value = activity.ActivityType;
                workSheet.Cells[i, 3].Value = activity.ContentTitle;
                workSheet.Cells[i, 4].Value = activity.Time;

                i++;
            }

            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            workSheet.Column(6).AutoFit();
            workSheet.Column(7).AutoFit();
            workSheet.Column(8).AutoFit();
            workSheet.Column(9).AutoFit();

            return package;
        }
    }
}