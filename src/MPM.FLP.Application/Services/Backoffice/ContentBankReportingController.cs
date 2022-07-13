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
    public class ContentBankReportingController : FLPAppServiceBase, IContentBankReportingController
    {
        private readonly ContentBankReportingAppService _appService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager _userManager;

        public ContentBankReportingController(ContentBankReportingAppService appService, IHostingEnvironment hostingEnvironment, UserManager userManager)
        {
            _appService = appService;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        [HttpGet("/api/services/app/backoffice/ContentBankReporting/ExportExcelSosmed")]
        public ActionResult ExportExcelSosmed(string channel = "", string search = "")
        {
            DateTime now = DateTime.Now;
            string excelName = "ContentBankReportingSosmed-"+now+".xlsx";

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("ContentBankReportingSosmed");

                var task = Task.Run(() => _appService.ExportExcelSosmed(channel, search));

                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "Tanggal Distribusi Kontent";
                workSheet.Cells[1, 2].Value = "Judul Konten";
                workSheet.Cells[1, 3].Value = "ID User";
                workSheet.Cells[1, 4].Value = "Kode Dealer";
                workSheet.Cells[1, 5].Value = "Dealer/FLP";
                workSheet.Cells[1, 6].Value = "Nama Dealer/FLP";
                workSheet.Cells[1, 7].Value = "Kota";
                workSheet.Cells[1, 8].Value = "Karesidenan";
                workSheet.Cells[1, 9].Value = "Download Date";
                workSheet.Cells[1, 10].Value = "Upload WA";
                workSheet.Cells[1, 11].Value = "Total Views Story WA";
                workSheet.Cells[1, 12].Value = "Tgl Upload FB";
                workSheet.Cells[1, 13].Value = "Link Upload FB";
                workSheet.Cells[1, 14].Value = "Total Reach Posting FB";
                workSheet.Cells[1, 15].Value = "Tgl Upload IG";
                workSheet.Cells[1, 16].Value = "Link Upload IG";
                workSheet.Cells[1, 17].Value = "Total Reach Posting IG";

                int row = 2;
                foreach (var result in task.Result)
                {
                    workSheet.Cells[row, 1].Value = result.CreationTime;
                    workSheet.Cells[row, 2].Value = result.Name;
                    workSheet.Cells[row, 3].Value = result.Username;
                    workSheet.Cells[row, 4].Value = result.KodeDealerAHM;
                    workSheet.Cells[row, 5].Value = result.KodeDealerAHM;
                    workSheet.Cells[row, 6].Value = result.NamaDealer;
                    workSheet.Cells[row, 7].Value = result.Kota;
                    workSheet.Cells[row, 8].Value = result.Karesidenan;
                    workSheet.Cells[row, 9].Value = result.DownloadDate;
                    workSheet.Cells[row, 10].Value = result.UploadWa;
                    workSheet.Cells[row, 11].Value = result.TotalViewWa;
                    workSheet.Cells[row, 12].Value = result.UploadDateFb;
                    workSheet.Cells[row, 13].Value = result.LinkFb;
                    workSheet.Cells[row, 14].Value = result.TotalViewFb;
                    workSheet.Cells[row, 15].Value = result.UploadDateIg;
                    workSheet.Cells[row, 16].Value = result.LinkIg;
                    workSheet.Cells[row, 17].Value = result.TotalViewIg;
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
                workSheet.Column(11).AutoFit();
                workSheet.Column(12).AutoFit();
                workSheet.Column(13).AutoFit();
                workSheet.Column(14).AutoFit();
                workSheet.Column(15).AutoFit();
                workSheet.Column(16).AutoFit();
                workSheet.Column(17).AutoFit();
                package.Save();
            }

            stream.Position = 0;
            
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") 
            { 
                FileDownloadName = excelName
            };        
        }

        [HttpGet("/api/services/app/backoffice/ContentBankReporting/ExportExcelDownload")]
        public ActionResult ExportExcelDownload(string channel = "", string search = "")
        {
            DateTime now = DateTime.Now;
            string excelName = "ContentBankReportingDownload-" + now + ".xlsx"; 
            
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("ContentBankReportingDownload");

                var task = Task.Run(() => _appService.ExportExcelDownload(channel, search));

                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "Karesidenan";
                workSheet.Cells[1, 2].Value = "Kode Dealer";
                workSheet.Cells[1, 3].Value = "Nama Dealer";
                workSheet.Cells[1, 4].Value = "Judul Konten";
                workSheet.Cells[1, 5].Value = "Status";
                
                int row = 2;
                foreach (var result in task.Result)
                {
                    workSheet.Cells[row, 1].Value = result.Karesidenan;
                    workSheet.Cells[row, 2].Value = result.KodeDealerAHM;
                    workSheet.Cells[row, 3].Value = result.NamaDealer;
                    workSheet.Cells[row, 4].Value = result.Content;
                    workSheet.Cells[row, 5].Value = result.Status;
                    row++;
                }

                workSheet.Column(1).AutoFit();
                workSheet.Column(2).AutoFit();
                workSheet.Column(3).AutoFit();
                workSheet.Column(4).AutoFit();
                workSheet.Column(5).AutoFit();
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