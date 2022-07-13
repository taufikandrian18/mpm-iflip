using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using MPM.FLP.Services.Dto;
using MPM.FLP.Common.Helpers;

namespace MPM.FLP.Services.Backoffice
{
    public class SalesTargetController : FLPAppServiceBase, ISalesTargetController
    {
        private readonly SalesTargetAppService _appService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SalesTargetController(
            IHostingEnvironment hostingEnvironment,
            SalesTargetAppService appService)
        {
            _hostingEnvironment = hostingEnvironment;
            _appService = appService;
        }

        [HttpPost("/api/services/app/backoffice/SalesTarget/downloadSalesTarget")]
        public ActionResult DownloadTemplateSalesTarget([FromBody] DownloadSalesTargetTemplateDto input)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Sales Target Template.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("SalesTargetTemplate");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Height = 20;
                    workSheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(2).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "";
                    workSheet.Cells[1, 2].Value = "";
                    workSheet.Cells[1, 3].Value = "";
                    workSheet.Cells[1, 4].Value = "";
                    workSheet.Cells[1, 5].Value = "";
                    workSheet.Cells[1, 6].Value = "Periode 1";
                    workSheet.Cells["F1:G1"].Merge = true;
                    workSheet.Cells[1, 8].Value = "Periode 2";
                    workSheet.Cells["H1:I1"].Merge = true;
                    workSheet.Cells[1, 10].Value = "Periode 3";
                    workSheet.Cells["J1:K1"].Merge = true;
                    workSheet.Cells[1, 12].Value = "Periode 4";
                    workSheet.Cells["L1:M1"].Merge = true;
                    workSheet.Cells[1, 14].Value = "Periode 5";
                    workSheet.Cells["N1:O1"].Merge = true;
                    workSheet.Cells[1, 16].Value = "Periode 6";
                    workSheet.Cells["P1:Q1"].Merge = true;

                    workSheet.Cells[2, 1].Value = "No";
                    workSheet.Cells[2, 2].Value = "Kota";
                    workSheet.Cells[2, 3].Value = "Kode Dealer";
                    workSheet.Cells[2, 4].Value = "Nama Dealer";
                    workSheet.Cells[2, 5].Value = "Target";
                    workSheet.Cells[2, 6].Value = input.Periode1Start == null ? "" : input.Periode1Start.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 7].Value = input.Periode1End == null ? "" : input.Periode1End.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 8].Value = input.Periode2Start == null ? "" : input.Periode2Start.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 9].Value = input.Periode2End == null ? "" : input.Periode2End.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 10].Value = input.Periode3Start == null ? "" : input.Periode3Start.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 11].Value = input.Periode3End == null ? "" : input.Periode3End.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 12].Value = input.Periode4Start == null ? "" : input.Periode4Start.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 13].Value = input.Periode4End == null ? "" : input.Periode4End.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 14].Value = input.Periode5Start == null ? "" : input.Periode5Start.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 15].Value = input.Periode5End == null ? "" : input.Periode5End.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 16].Value = input.Periode6Start == null ? "" : input.Periode6Start.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[2, 17].Value = input.Periode6End == null ? "" : input.Periode6End.Value.ToString("yyyy-MM-dd");

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

        [HttpPost("/api/services/app/backoffice/SalesTarget/importSalesTarget")]
        public async Task<String> ImportExcelSalesTarget(int Month, int Year, string CreatorUsername, [FromForm] IEnumerable<IFormFile> files)
        {
            List<TargetSales> TargetSalesList = new List<TargetSales>();

            if (files.Count() > 0)
            {
                var file = files.FirstOrDefault();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["SalesTargetTemplate"];
                        var rowCount = worksheet.Dimension.Rows;

                        DateTime? Periode1Start;
                        DateTime? Periode1End;
                        DateTime? Periode2Start;
                        DateTime? Periode2End;
                        DateTime? Periode3Start;
                        DateTime? Periode3End;
                        DateTime? Periode4Start;
                        DateTime? Periode4End;
                        DateTime? Periode5Start;
                        DateTime? Periode5End;
                        DateTime? Periode6Start;
                        DateTime? Periode6End;

                        string tmp = string.Empty;
                        bool isValidDate = false;

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 6].Value?.ToString().Trim(), out Periode1Start);
                        if (!isValidDate) return "Import Failed. Periode 1 Start must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 7].Value?.ToString().Trim(), out Periode1End);
                        if (!isValidDate) return "Import Failed. Periode 1 End must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 8].Value?.ToString().Trim(), out Periode2Start);
                        if (!isValidDate) return "Import Failed. Periode 2 Start must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 9].Value?.ToString().Trim(), out Periode2End);
                        if (!isValidDate) return "Import Failed. Periode 2 End must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 10].Value?.ToString().Trim(), out Periode3Start);
                        if (!isValidDate) return "Import Failed. Periode 3 Start must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 11].Value?.ToString().Trim(), out Periode3End);
                        if (!isValidDate) return "Import Failed. Periode 3 End must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 12].Value?.ToString().Trim(), out Periode4Start);
                        if (!isValidDate) return "Import Failed. Periode 4 Start must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 13].Value?.ToString().Trim(), out Periode4End);
                        if (!isValidDate) return "Import Failed. Periode 4 End must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 14].Value?.ToString().Trim(), out Periode5Start);
                        if (!isValidDate) return "Import Failed. Periode 5 Start must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 15].Value?.ToString().Trim(), out Periode5End);
                        if (!isValidDate) return "Import Failed. Periode 5 End must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 16].Value?.ToString().Trim(), out Periode6Start);
                        if (!isValidDate) return "Import Failed. Periode 6 Start must be valid date.";

                        isValidDate = ParsingHelper.TryParseNullableDateTime(worksheet.Cells[2, 17].Value?.ToString().Trim(), out Periode6End);
                        if (!isValidDate) return "Import Failed. Periode 6 End must be valid date.";

                        for (int row = 3; row <= rowCount; row++)
                        {
                            var dealerCode = worksheet.Cells[row, 3].Value?.ToString().Trim();
                            var target = worksheet.Cells[row, 5].Value?.ToString().Trim();
                            var target1 = worksheet.Cells[row, 7].Value?.ToString().Trim();
                            var target2 = worksheet.Cells[row, 9].Value?.ToString().Trim();
                            var target3 = worksheet.Cells[row, 11].Value?.ToString().Trim();
                            var target4 = worksheet.Cells[row, 13].Value?.ToString().Trim();
                            var target5 = worksheet.Cells[row, 15].Value?.ToString().Trim();
                            var target6 = worksheet.Cells[row, 17].Value?.ToString().Trim();

                            if (dealerCode == null) return "Import Failed. Field Dealer Code at row " + row + " must be filled.";
                            if (target == null) return "Import Failed. Field Target at row " + row + " must be filled.";

                            TargetSalesList.Add(new TargetSales
                            {
                                DealerId = dealerCode,
                                Month = Month,
                                Year = Year,
                                Periode1Start = Periode1Start,
                                Periode1End = Periode1End,
                                Periode1Target = string.IsNullOrEmpty(target1) ? 0 : int.Parse(target1),
                                Periode2Start = Periode2Start,
                                Periode2End = Periode2End,
                                Periode2Target = string.IsNullOrEmpty(target2) ? 0 : int.Parse(target2),
                                Periode3Start = Periode3Start,
                                Periode3End = Periode3End,
                                Periode3Target = string.IsNullOrEmpty(target3) ? 0 : int.Parse(target3),
                                Periode4Start = Periode4Start,
                                Periode4End = Periode4End,
                                Periode4Target = string.IsNullOrEmpty(target4) ? 0 : int.Parse(target4),
                                Periode5Start = Periode5Start,
                                Periode5End = Periode5End,
                                Periode5Target = string.IsNullOrEmpty(target5) ? 0 : int.Parse(target5),
                                Periode6Start = Periode6Start,
                                Periode6End = Periode6End,
                                Periode6Target = string.IsNullOrEmpty(target6) ? 0 : int.Parse(target6),
                                TargetTotal = int.Parse(target),
                                CreatorUsername = CreatorUsername,
                                CreationTime = DateTime.Now
                            });
                        }
                        _appService.CreateMultiple(TargetSalesList);
                    }
                }
            }
            return "Success Import";
        }
    }
}