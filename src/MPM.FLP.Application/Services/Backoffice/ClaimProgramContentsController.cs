using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services.Dto;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MPM.FLP.Services.Backoffice
{
    public class ClaimProgramContentsController : FLPAppServiceBase, IClaimProgramContentsController
    {
        private readonly ClaimProgramContentClaimerAppService _appService;
        
        public ClaimProgramContentsController(ClaimProgramContentClaimerAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/ClaimProgramContents/ExportExcel")]
        public ActionResult ExportExcel(FilterGetClaimerDto request)
        {
            DateTime now = DateTime.Now;
            string excelName = "ClaimProgramContents-" + now + ".xlsx";

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("ClaimProgramContents");

                var task = Task.Run(() => _appService.ExportExcel(request));

                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "Username";
                workSheet.Cells[1, 2].Value = "Nama FLP";
                workSheet.Cells[1, 3].Value = "No HP";
                workSheet.Cells[1, 4].Value = "Jabatan";
                workSheet.Cells[1, 5].Value = "Nama Toko";
                workSheet.Cells[1, 6].Value = "Kota";
                workSheet.Cells[1, 7].Value = "Foto";
                workSheet.Cells[1, 8].Value = "Approved";
                workSheet.Cells[1, 9].Value = "Is Verified";
                workSheet.Cells[1, 10].Value = "Kode OTP";
                
                int row = 2;
                foreach (var result in task.Result)
                {
                    workSheet.Cells[row, 1].Value = result.ClaimerUsername;
                    workSheet.Cells[row, 2].Value = result.NamaDealer;
                    workSheet.Cells[row, 3].Value = result.Handphone;
                    //workSheet.Cells[row, 4].Value = result.Jabatan;
                    //workSheet.Cells[row, 5].Value = result.ShopName;
                    workSheet.Cells[row, 6].Value = result.KotaDealer;
                    workSheet.Cells[row, 7].Value = result.StorageUrl;
                    workSheet.Cells[row, 8].Value = result.IsApproved;
                    workSheet.Cells[row, 9].Value = result.IsVerified;
                    workSheet.Cells[row, 10].Value = result.OTP;
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