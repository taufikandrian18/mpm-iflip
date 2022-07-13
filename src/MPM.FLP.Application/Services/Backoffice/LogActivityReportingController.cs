using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using MPM.FLP.Services.Dto;
using MPM.FLP.Authorization.Users;
using Newtonsoft.Json;

namespace MPM.FLP.Services.Backoffice
{
    public class LogActivityReportingController : FLPAppServiceBase, ILogActivityReportingController
    {
        private readonly LogActivityReportingAppService _appService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager _userManager;

        public LogActivityReportingController(LogActivityReportingAppService appService, IHostingEnvironment hostingEnvironment, UserManager userManager)
        {
            _appService = appService;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        [HttpGet("/api/services/app/backoffice/LogActivityReporting/ExportExcelDetail")]
        public ActionResult ExportExcelDetail(LogActivityReportingFilterDto request)
        {
            DateTime now = DateTime.Now;
            string excelName = "LogActivityDetail-"+now+".csv";

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Data");

                var data = _appService.ExportExcelDetail(request);

                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "IDMPM";
                workSheet.Cells[1, 2].Value = "Name";
                workSheet.Cells[1, 3].Value = "Action";
                workSheet.Cells[1, 4].Value = "Date";
                workSheet.Cells[1, 5].Value = "Old Data";
                workSheet.Cells[1, 6].Value = "Result Data";

                int row = 2;
                foreach (var result in data)
                {
                    workSheet.Cells[row, 1].Value = result.IDMPM;
                    workSheet.Cells[row, 2].Value = result.Name;
                    workSheet.Cells[row, 3].Value = result.Action;
                    workSheet.Cells[row, 4].Value = result.Date;
                    workSheet.Cells[row, 5].Value = result.OldData;
                    workSheet.Cells[row, 6].Value = result.ResultData;
                    row++;
                }

                workSheet.Column(1).AutoFit();
                workSheet.Column(2).AutoFit();
                workSheet.Column(3).AutoFit();
                workSheet.Column(4).AutoFit();
                workSheet.Column(5).AutoFit();
                workSheet.Column(6).AutoFit();
                package.Save();
            }

            stream.Position = 0;
            
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") 
            { 
                FileDownloadName = excelName
            };        
        }

        [HttpGet("/api/services/app/backoffice/ContentBankReporting/ExportExcelSummary")]
        public ActionResult ExportExcelSummary(LogActivityReportingFilterDto request)
        {
            DateTime now = DateTime.Now;
            string excelName = "LogActivitySummary-" + now + ".csv"; 
            
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Data");

                var data = _appService.ExportExcelSummary(request);

                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "Page Name";
                workSheet.Cells[1, 2].Value = "Action";
                workSheet.Cells[1, 3].Value = "Total";
                
                int row = 2;
                foreach (var result in data)
                {
                    workSheet.Cells[row, 1].Value = result.PageName;
                    workSheet.Cells[row, 2].Value = result.Action;
                    workSheet.Cells[row, 3].Value = result.Total;
                    row++;
                }

                workSheet.Column(1).AutoFit();
                workSheet.Column(2).AutoFit();
                workSheet.Column(3).AutoFit();
                package.Save();
            }

            stream.Position = 0;

            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = excelName
            };
        }
    }
}