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
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Web;
using MPM.FLP.Services.Dto;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.DataValidation;
using Abp.Web.Models;
using MPM.FLP.Web.Models.FLPMPM;
using Newtonsoft.Json;
using Kendo.Mvc;
using Abp.Runtime.Validation;
using Abp.Extensions;

namespace MPM.FLP.Web.Mvc.Controllers
{
    public class SelfRecordingController : FLPControllerBase
    {
        private readonly SelfRecordingAppService _appService;
        private readonly SelfRecordingDetailAppService _detailAppService;
        private readonly SelfRecordingResultAppService _resultAppService;
        private readonly SelfRecordingResultDetailAppService _detailResultAppService;
        private readonly SelfRecordingAssignmentAppService _assignmentAppService;
        private readonly DealerAppService _dealerAppService;
        private readonly KotaAppService _kotaAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SelfRecordingController(SelfRecordingAppService appService, SelfRecordingDetailAppService detailAppService, SelfRecordingResultAppService resultAppService, SelfRecordingResultDetailAppService detailResultAppService, SelfRecordingAssignmentAppService assignmentAppService, DealerAppService dealerAppService, KotaAppService kotaAppService, IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _detailAppService = detailAppService;
            _resultAppService = resultAppService;
            _detailResultAppService = detailResultAppService;
            _hostingEnvironment = hostingEnvironment;
            _assignmentAppService = assignmentAppService;
            _dealerAppService = dealerAppService;
            _kotaAppService = kotaAppService;
        }

        #region Main

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            SelfRecordings model = new SelfRecordings();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SelfRecordings model, string submit, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";

                _appService.Create(model);
            }
            return Json(new { success = true });
        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetById(id);

            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(SelfRecordings model, string submit)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return Json(new { success = true });
        }

        public IActionResult Grid_Read([DataSourceRequest] DataSourceRequest request)
        {

            DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest] DataSourceRequest request, ClaimPrograms item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Detail
        public IActionResult Detail(Guid id)
        {
            var model = _appService.GetById(id);
            return View(model);
        }

        public IActionResult BackToDetail(SelfRecordingDetails item)
        {
            var model = _appService.GetById(item.SelfRecordingId);
            return Detail(model.Id);
        }

        public IActionResult CreateDetail(Guid id)
        {
            SelfRecordingDetails model = new SelfRecordingDetails();
            model.SelfRecordingId = id;
            model.IsMandatorySilver = false;
            model.IsMandatoryGold = false;
            model.IsMandatoryPlatinum = false;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDetail(SelfRecordingDetails model, string submit)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";

                _detailAppService.Create(model);
            }
            return Json(new { success = true, id = model.SelfRecordingId });
        }

        public IActionResult EditDetail(Guid id)
        {
            var item = _detailAppService.GetById(id);

            return View(item);
        }

        [HttpPost]
        public IActionResult EditDetail(SelfRecordingDetails model, string submit)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                _detailAppService.Update(model);
            }
            return Json(new { success = true, id = model.SelfRecordingId });
        }

        public IActionResult Grid_Detail_Read([DataSourceRequest] DataSourceRequest request, SelfRecordings item)
        {
            DataSourceResult result = _detailAppService.GetAll().Where(x => x.SelfRecordingId == item.Id && string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Detail_Destroy([DataSourceRequest] DataSourceRequest request, RolePlayDetails item)
        {
            if (ModelState.IsValid)
            {
                _detailAppService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Result

        public IActionResult Result(Guid id)
        {
            //var model = _appService.GetById(id);
            var tmp = _assignmentAppService.GetById(id);
            AssignmentDealerVM model = new AssignmentDealerVM
            {
                Id = tmp.SelfRecordingId,
                Title = _appService.GetById(tmp.SelfRecordingId).Title,
                KodeDealerMPM = tmp.KodeDealerMPM,
                NamaDealer = tmp.NamaDealer
            };
            return View("Result", model);
        }
        public IActionResult BackToResult(Guid id)
        {
            var result = _resultAppService.GetAll().SingleOrDefault(x => x.Id == id);
            var model = _appService.GetById(result.SelfRecordingId);
            var dealer = _assignmentAppService.GetAll().SingleOrDefault(x => x.SelfRecordingId == model.Id && x.KodeDealerMPM == result.KodeDealerMPM);
            //return RedirectToAction("Result",model);
            return Result(dealer.Id);
        }
        [DontWrapResult(WrapOnError = false, WrapOnSuccess = false)]
        public ActionResult Grid_Result_Read([DataSourceRequest] DataSourceRequest request, AssignmentDealerVM item)
        {
            var tmp = _resultAppService.GetAll().Where(x => x.SelfRecordingId == item.Id && x.KodeDealerMPM == item.KodeDealerMPM && string.IsNullOrEmpty(x.DeleterUsername))
                    .GroupBy(x => new { x.IDMPM, x.NamaFLP })
                    .Select(x => new RoleplayResultVM
                    {
                        idmpm = x.Key.IDMPM,
                        namaFLP = x.Key.NamaFLP,
                        flpResult = x.Max(y => y.FLPResult),
                        verificationResult = x.OrderByDescending(y => y.CreationTime).FirstOrDefault().VerificationResult,
                        url = (x.OrderByDescending(y => y.CreationTime).FirstOrDefault().StorageUrl == "" || x.OrderByDescending(y => y.CreationTime).FirstOrDefault().StorageUrl == null) ? x.OrderByDescending(y => y.CreationTime).FirstOrDefault().YoutubeUrl : x.OrderByDescending(y => y.CreationTime).FirstOrDefault().StorageUrl
                    }
            ).ToList();
            string serializer = JsonConvert.SerializeObject(tmp.ToDataSourceResult(request));
            //var result = new ContentResult();

            //result.Content = serializer;
            //result.ContentType = "aspnetmvc-ajax";

            DataSourceResult result = tmp.ToDataSourceResult(request);

            return Content(serializer, "application/json");
            //return Json(result);
        }

        [DontWrapResult(WrapOnError = false, WrapOnSuccess = false)]
        public IActionResult Grid_Result_Detail_Read([DataSourceRequest] DataSourceRequest request, string idmpm, Guid idRoleplay)
        {
            var id = int.Parse(idmpm);
            //DataSourceResult result = _resultAppService.GetAll().Where(x => x.IDMPM == id && string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

            //return Json(result);
            var tmp = _resultAppService.GetAll().Where(x => x.IDMPM == id && string.IsNullOrEmpty(x.DeleterUsername) && x.SelfRecordingId == idRoleplay)
                    .Select(x => new RoleplayResultVM
                    {
                        id = x.Id,
                        idmpm = x.IDMPM,
                        namaFLP = x.NamaFLP,
                        flpResult = x.FLPResult,
                        isVerified = x.IsVerified,
                        kodeDealerMPM = x.KodeDealerMPM,
                        namaDealerMPM = x.NamaDealerMPM,
                        verificationResult = x.VerificationResult,
                        CreationTime = x.CreationTime
                    }
            ).OrderByDescending(x => x.CreationTime).ToList();
            string serializer = JsonConvert.SerializeObject(tmp.ToDataSourceResult(request));

            return Content(serializer, "application/json");
        }

        #endregion

        #region Validation

        public IActionResult Validation(Guid id)
        {
            var model = _resultAppService.GetById(id);
            return View(model);
        }
        public IActionResult GetUrl(Guid id)
        {
            var model = _resultAppService.GetById(id);
            var url = (model.StorageUrl == "" || model.StorageUrl == null) ? model.YoutubeUrl : model.StorageUrl;
            bool isYoutube = (model.StorageUrl == "" || model.StorageUrl == null) ? true : false;
            return Json(new { url = url, isYoutube = isYoutube });
        }

        [HttpPost]
        public IActionResult CalculatePoint([FromBody] List<ValidationVM> validations, Guid id)
        {
            double total = 0;
            double totalDetail = 0;
            List<SelfRecordingResultDetails> listPass = new List<SelfRecordingResultDetails>();
            List<string> listNotPass = new List<string>();
            List<string> listDismiss = new List<string>();
            List<SelfRecordingDetails> listDetail = new List<SelfRecordingDetails>();

            bool containSilver = false;
            bool containGold = false;
            bool containPlatinum = false;

            int totalPlatinum = 0;
            int totalGold = 0;
            int totalSilver = 0;

            foreach (var validation in validations)
            {
                //var detail = _detailAppService.GetById(Guid.Parse(validation.Id));
                //var result = _detailResultAppService.GetAll().SingleOrDefault(x => x.Title == detail.Title && x.SelfRecordingResultId == validation.ResultId);
                var result = _detailResultAppService.GetAll().SingleOrDefault(x => x.SelfRecordingDetailId == Guid.Parse(validation.Id) && x.SelfRecordingResultId == validation.ResultId);

                if (totalDetail == 0)
                {
                    var tmpResult = _detailResultAppService.GetAll().Where(x => x.SelfRecordingResultId == validation.ResultId).ToList();

                    foreach (var tmpresult in tmpResult)
                    {
                        totalDetail++;
                    }

                    totalPlatinum = _detailResultAppService.GetAll().Where(x => x.IsMandatoryPlatinum == true && x.SelfRecordingResultId == validation.ResultId).Count();
                    totalGold = _detailResultAppService.GetAll().Where(x => x.IsMandatoryGold == true && x.SelfRecordingResultId == validation.ResultId).Count();
                    totalSilver = _detailResultAppService.GetAll().Where(x => x.IsMandatorySilver == true && x.SelfRecordingResultId == validation.ResultId).Count();
                }


                if (validation.Condition == "pass")
                {
                    listPass.Add(result);
                }
                if (validation.Condition == "notpass")
                {
                    listNotPass.Add(validation.Id);
                }
                if (validation.Condition == "dismiss")
                {
                    listDismiss.Add(validation.Id);
                }


            }

            containSilver = listPass.Where(x => x.IsMandatorySilver == true).Count() > 0 ? true : false;
            containGold = listPass.Where(x => x.IsMandatoryGold == true).Count() > 0 ? true : false;
            containPlatinum = listPass.Where(x => x.IsMandatoryPlatinum == true).Count() > 0 ? true : false;

            if (listPass.Count > 0)
            {
                //total = ((double)listPass.Count / ((double)listDetail.Count - (double)listDismiss.Count)) * 100;
                total = ((double)listPass.Count / (totalDetail - (double)listDismiss.Count)) * 100;

                if (total != 100)
                {
                    if (totalPlatinum > 0)
                    {
                        if (total >= 90 && listPass.Where(x => x.IsMandatoryPlatinum == true).Count() < totalPlatinum && (listPass.Where(x => x.IsMandatorySilver == true).Count() == 0 || listPass.Where(x => x.IsMandatoryGold == true).Count() == 0))
                        {
                            total = 89.99;
                        }
                        else if (total >= 50 && (listPass.Where(x => x.IsMandatorySilver == true).Count() > 0 || listPass.Where(x => x.IsMandatoryGold == true).Count() > 0))
                        {
                            total = 49.99;
                        }
                    }
                    else if (totalGold > 0)
                    {
                        if (total >= 90 && listPass.Where(x => x.IsMandatoryGold == true).Count() == totalGold)
                        {
                            total = 89.99;
                        }
                        else if ((total >= 90 || (total < 90 && total >= 70)) && listPass.Where(x => x.IsMandatoryGold == true).Count() < totalGold)
                        {
                            total = 69.99;
                        }
                    }
                    else if (totalSilver > 0)
                    {
                        if (total >= 70 && listPass.Where(x => x.IsMandatorySilver == true).Count() == totalSilver)
                        {
                            total = 69.99;
                        }
                        else if ((total >= 70 || (total < 70 && total >= 50)) && listPass.Where(x => x.IsMandatorySilver == true).Count() < totalSilver)
                        {
                            total = 49.99;
                        }
                    }
                }

            }

            return Json(new { value = Math.Round(total, 2) });
        }

        public async Task<IActionResult> GetMandatory(Guid id)
        {
            int totalPlatinum = 0;
            int totalGold = 0;
            int totalSilver = 0;

            var detail = _detailResultAppService.GetAll().Where(x => x.SelfRecordingResultId == id).ToList();

            totalPlatinum = detail.Where(x => x.IsMandatoryPlatinum == true).Count();
            totalGold = detail.Where(x => x.IsMandatoryGold == true).Count();
            totalSilver = detail.Where(x => x.IsMandatorySilver == true).Count();

            if (totalPlatinum > 0 && totalGold > 0 && totalSilver > 0)
            {
                return Json(new { value = "Bronze" });
            }
            else if (totalPlatinum > 0 && totalGold > 0)
            {
                return Json(new { value = "Silver" });
            }
            else if (totalPlatinum > 0)
            {
                return Json(new { value = "Gold" });
            }
            else if (totalGold > 0)
            {
                return Json(new { value = "Silver" });
            }
            else if (totalSilver > 0)
            {
                return Json(new { value = "Bronze" });
            }
            else
            {
                return Json(new { value = "Platinum" });
            }
        }

        [DontWrapResult(WrapOnError = false, WrapOnSuccess = false)]
        public ActionResult Grid_Validate_Read([DataSourceRequest] DataSourceRequest request, SelfRecordingResults item)
        {
            List<RoleplayResultDetailVM> details = new List<RoleplayResultDetailVM>();
            //var tmp = _detailAppService.GetAll().Where(x => x.SelfRecordingId == item.SelfRecordingId && string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.CreationTime)
            //        .Select(x => new RoleplayResultDetailVM
            //        {
            //            id = x.Id,
            //            title = x.Title,
            //            order = x.Order,
            //            rolePlayId = x.SelfRecordingId,
            //            rolePlayResultId = item.Id
            //        }
            //).ToList();

            var tmpResult = _detailResultAppService.GetAll().Where(x => x.SelfRecordingResultId == item.Id).ToList();

            foreach (var tmpresult in tmpResult)
            {
                //var detail = _detailAppService.GetAll().SingleOrDefault(x => x.SelfRecordingId == item.SelfRecordingId && string.IsNullOrEmpty(x.DeleterUsername) && tmpresult.Title == x.Title);
                var detail = _detailAppService.GetAll().SingleOrDefault(x => x.SelfRecordingId == item.SelfRecordingId && tmpresult.SelfRecordingDetailId == x.Id);

                if (detail != null)
                {
                    details.Add(new RoleplayResultDetailVM
                    {
                        id = detail.Id,
                        title = detail.Title,
                        order = detail.Order,
                        rolePlayId = detail.SelfRecordingId,
                        rolePlayResultId = item.Id,

                        beforePassed = tmpresult.BeforePassed,
                        beforeNotPassed = tmpresult.BeforeNotPassed,
                        beforeDismiss = tmpresult.BeforeDismiss,

                        afterPassed = tmpresult.AfterPassed,
                        afterNotPassed = tmpresult.AfterNotPassed,
                        afterDismiss = tmpresult.AfterDismiss,

                        isMandatorySilver = tmpresult.IsMandatorySilver,
                        isMandatoryGold = tmpresult.IsMandatoryGold,
                        isMandatoryPlatinum = tmpresult.IsMandatoryPlatinum
                    });
                }
                //else
                //{
                //    detail = _detailAppService.GetAll().FirstOrDefault(x => x.SelfRecordingId == item.SelfRecordingId && !string.IsNullOrEmpty(x.DeleterUsername) && tmpresult.Title == x.Title);
                //    var tmpId = _resultAppService.GetById(tmpresult.SelfRecordingResultId).SelfRecordingId;
                //    details.Add(new RoleplayResultDetailVM
                //    {
                //        id = detail.Id,
                //        title = tmpresult.Title,
                //        order = tmpresult.Order,
                //        rolePlayId = tmpId,
                //        rolePlayResultId = tmpresult.SelfRecordingResultId
                //    });
                //}

            }

            //foreach (var value in details)
            //{
            //    var detail = tmpResult.SingleOrDefault(x => x.SelfRecordingResultId == value.rolePlayResultId && x.Title == value.title);

            //    if (detail != null)
            //    {
            //        value.beforePassed = detail.BeforePassed;
            //        value.beforeNotPassed = detail.BeforeNotPassed;
            //        value.beforeDismiss = detail.BeforeDismiss;

            //        value.afterPassed = detail.AfterPassed;
            //        value.afterNotPassed = detail.AfterNotPassed;
            //        value.afterDismiss = detail.AfterDismiss;

            //        value.isMandatorySilver = detail.IsMandatorySilver;
            //        value.isMandatoryGold = detail.IsMandatoryGold;
            //        value.isMandatoryPlatinum = detail.IsMandatoryPlatinum;
            //    }

            //}
            string serializer = JsonConvert.SerializeObject(details.OrderBy(x => x.order).ToDataSourceResult(request));

            return Content(serializer, "application/json");
            //return Json(result);
        }

        [HttpPost]
        public IActionResult Validate([DataSourceRequest] DataSourceRequest request, List<string> pass, List<string> notpass, List<string> dismiss, string id, decimal resultVaildation)
        {
            SelfRecordingResults result = _resultAppService.GetById(Guid.Parse(id));

            ValidateResult(pass, notpass, dismiss, id, resultVaildation);

            result.IsVerified = true;
            result.VerificationResult = resultVaildation;
            _resultAppService.Validate(result);

            return Json(new { success = "true" });
        }

        [HttpPost]
        public IActionResult SaveValidation([DataSourceRequest] DataSourceRequest request, List<string> pass, List<string> notpass, List<string> dismiss, string id, decimal resultVaildation)
        {
            ValidateResult(pass, notpass, dismiss, id, resultVaildation);

            return Json(new { success = "true" });
        }

        public void ValidateResult(List<string> pass, List<string> notpass, List<string> dismiss, string id, decimal resultVaildation)
        {
            SelfRecordingResults result = _resultAppService.GetById(Guid.Parse(id));
            SelfRecordingResultDetails resultDetail = new SelfRecordingResultDetails();
            foreach (var item in pass)
            {
                SelfRecordingDetails details = _detailAppService.GetById(Guid.Parse(item));
                resultDetail = _detailResultAppService.GetAll().SingleOrDefault(x => x.Title == details.Title && x.Order == details.Order && x.SelfRecordingResultId == result.Id);
                if (resultDetail != null)
                {
                    resultDetail.AfterPassed = true;
                    resultDetail.AfterDismiss = false;
                    resultDetail.AfterNotPassed = false;
                    resultDetail.LastModificationTime = DateTime.UtcNow.AddHours(7);
                    resultDetail.LastModifierUsername = this.User.Identity.Name;
                    _detailResultAppService.Update(resultDetail);
                }

            }
            foreach (var item in notpass)
            {
                SelfRecordingDetails details = _detailAppService.GetById(Guid.Parse(item));
                resultDetail = _detailResultAppService.GetAll().SingleOrDefault(x => x.Title == details.Title && x.Order == details.Order && x.SelfRecordingResultId == result.Id);
                if (resultDetail != null)
                {
                    resultDetail.AfterPassed = false;
                    resultDetail.AfterDismiss = false;
                    resultDetail.AfterNotPassed = true;
                    resultDetail.LastModificationTime = DateTime.UtcNow.AddHours(7);
                    resultDetail.LastModifierUsername = this.User.Identity.Name;
                    _detailResultAppService.Update(resultDetail);
                }
            }
            foreach (var item in dismiss)
            {
                SelfRecordingDetails details = _detailAppService.GetById(Guid.Parse(item));
                resultDetail = _detailResultAppService.GetAll().SingleOrDefault(x => x.Title == details.Title && x.Order == details.Order && x.SelfRecordingResultId == result.Id);
                if (resultDetail != null)
                {
                    resultDetail.AfterPassed = false;
                    resultDetail.AfterDismiss = true;
                    resultDetail.AfterNotPassed = false;
                    resultDetail.LastModificationTime = DateTime.UtcNow.AddHours(7);
                    resultDetail.LastModifierUsername = this.User.Identity.Name;
                    _detailResultAppService.Update(resultDetail);
                }

            }

        }
        #endregion

        #region Excel

        public ActionResult DownloadDetail(SelfRecordings model)
        {
            model = _appService.GetById(model.Id);
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Self Recording " + model.Title + " Detail.xlsx";

            var details = _detailAppService.GetAll().Where(x => x.SelfRecordingId == model.Id).ToList();

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Self Recording Detail");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Judul";
                    workSheet.Cells[1, 2].Value = "Mandatory";
                    workSheet.Cells[1, 3].Value = "Pembuat";
                    workSheet.Cells[1, 4].Value = "Tanggal Buat";
                    workSheet.Cells[1, 5].Value = "Pembaharu";
                    workSheet.Cells[1, 6].Value = "Tanggal Pembaharuan";

                    int row = 2;
                    foreach (var item in details)
                    {
                        string mandatory = "";
                        if (item.IsMandatorySilver)
                        {
                            mandatory += " S";
                        }
                        if (item.IsMandatoryGold)
                        {
                            mandatory += " G";
                        }
                        if (item.IsMandatoryPlatinum)
                        {
                            mandatory += " P";
                        }
                        workSheet.Cells[row, 1].Value = item.Title;
                        workSheet.Cells[row, 2].Value = mandatory;
                        workSheet.Cells[row, 3].Value = item.CreatorUsername;
                        workSheet.Cells[row, 4].Value = item.CreationTime;
                        workSheet.Cells[row, 4].Style.Numberformat.Format = "yyyy-mm-dd";
                        workSheet.Cells[row, 5].Value = item.LastModifierUsername;
                        workSheet.Cells[row, 6].Value = item.LastModificationTime;
                        workSheet.Cells[row, 6].Style.Numberformat.Format = "yyyy-mm-dd";

                        row++;
                    }


                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();
                    workSheet.Column(6).AutoFit();


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

        public ActionResult DownloadTemplate(SelfRecordings model)
        {
            model = _appService.GetById(model.Id);
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Self Recording Detail Template.xlsx";

            var details = _detailAppService.GetAll().Where(x => x.SelfRecordingId == model.Id).ToList();

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Self Recording Detail Template");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Judul";
                    workSheet.Cells[1, 2].Value = "Mandatory Silver";
                    workSheet.Cells[1, 3].Value = "Mandatory Gold";
                    workSheet.Cells[1, 4].Value = "Mandatory Platinum";
                    workSheet.Cells[1, 5].Value = "Order";
                    workSheet.Cells["E2:E1000"].Style.Numberformat.Format = "0";

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();

                    var validation = workSheet.DataValidations.AddListValidation("B2:B1000");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    validation.ErrorTitle = "An invalid value was entered";
                    validation.Error = "Select a value from the list";
                    validation.Formula.Values.AddRange(new List<string> { "Yes", "No" });

                    validation = workSheet.DataValidations.AddListValidation("C2:C1000");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    validation.ErrorTitle = "An invalid value was entered";
                    validation.Error = "Select a value from the list";
                    validation.Formula.Values.AddRange(new List<string> { "Yes", "No" });

                    validation = workSheet.DataValidations.AddListValidation("D2:D1000");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    validation.ErrorTitle = "An invalid value was entered";
                    validation.Error = "Select a value from the list";
                    validation.Formula.Values.AddRange(new List<string> { "Yes", "No" });

                    package.Save();
                }
            }
            catch (Exception ex)
            {

            }

            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        public IActionResult ImportExcel(Guid id)
        {
            var item = _appService.GetById(id);
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(SelfRecordings model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var file = files.FirstOrDefault();
                    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewBag.message = "Format file hanya mendukung .xlsx";
                        return Redirect("~/SelfRecording/ImportExcel/" + model.Id);
                    }

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Self Recording Detail Template"];
                            if (worksheet == null)
                            {
                                TempData["alert"] = "Format template salah";
                                TempData["success"] = "";
                                return RedirectToAction("ImportExcel/" + model.Id);
                            }
                            var rowCount = worksheet.Dimension.Rows;
                            //TempData["alert"] = "Bahaya";
                            //return RedirectToAction("VarianHarga/" + model.Id);
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var judul = worksheet.Cells[row, 1].Value?.ToString().Trim();

                                bool silver = worksheet.Cells[row, 2].Value?.ToString().Trim().ToLower() == "yes" ? true : false;
                                bool gold = worksheet.Cells[row, 3].Value?.ToString().Trim().ToLower() == "yes" ? true : false;
                                bool platinum = worksheet.Cells[row, 4].Value?.ToString().Trim().ToLower() == "yes" ? true : false;
                                var order = worksheet.Cells[row, 5].Value != null ? int.Parse(worksheet.Cells[row, 5].Value.ToString().Trim()) : 0;

                                if (judul == null && silver == false && gold == false && platinum == false && order == 0) break;

                                if (judul == null)
                                {
                                    TempData["alert"] = "Judul pada baris " + row + " masih kosong";
                                    TempData["success"] = "";
                                    return RedirectToAction("ImportExcel/" + model.Id);
                                }
                                SelfRecordingDetails item = new SelfRecordingDetails
                                {
                                    Id = Guid.NewGuid(),
                                    Title = judul,
                                    Order = order,
                                    IsMandatorySilver = silver,
                                    IsMandatoryGold = gold,
                                    IsMandatoryPlatinum = platinum,
                                    CreationTime = DateTime.Now,
                                    CreatorUsername = this.User.Identity.Name,
                                    LastModifierUsername = this.User.Identity.Name,
                                    LastModificationTime = DateTime.Now,
                                    SelfRecordingId = model.Id,
                                    DeleterUsername = ""
                                };
                                _detailAppService.Create(item);
                            }
                        }
                    }
                }

                TempData["success"] = "Berhasil menambah data";
                TempData["alert"] = "";
            }
            return Redirect("~/SelfRecording/Detail/" + model.Id);
        }

        public ActionResult DownloadResults(string id, string title, string namaDealer)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Self Recording " + title + " Result.xlsx";

            var details = _resultAppService.GetAll().Where(x => x.SelfRecordingId == Guid.Parse(id) && x.NamaDealerMPM == namaDealer).OrderBy(x => x.IDMPM).ToList();

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Self Recording Result");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "ID MPM";
                    workSheet.Cells[1, 2].Value = "Nama FLP";
                    workSheet.Cells[1, 3].Value = "Kode Dealer";
                    workSheet.Cells[1, 4].Value = "Nama Dealer";
                    workSheet.Cells[1, 5].Value = "FLP Result";
                    workSheet.Cells[1, 6].Value = "Verification Result";
                    workSheet.Cells[1, 7].Value = "Verified";

                    int row = 2;
                    foreach (var item in details)
                    {
                        workSheet.Cells[row, 1].Value = item.IDMPM;
                        workSheet.Cells[row, 2].Value = item.NamaFLP;
                        workSheet.Cells[row, 3].Value = item.KodeDealerMPM;
                        workSheet.Cells[row, 4].Value = item.NamaDealerMPM;
                        workSheet.Cells[row, 5].Value = item.FLPResult;
                        workSheet.Cells[row, 6].Value = item.VerificationResult;
                        workSheet.Cells[row, 7].Value = item.IsVerified;

                        row++;
                    }


                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();
                    workSheet.Column(6).AutoFit();
                    workSheet.Column(7).AutoFit();

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

        public ActionResult DownloadAllExcel(Guid id)
        {
            var selfRecordings = _appService.GetById(id);
            var assignments = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id);


            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Self Recording " + selfRecordings.Title + ".xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Dealer");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Kode Dealer MPM";
                    workSheet.Cells[1, 2].Value = "Nama Dealer";

                    int row = 2;
                    foreach (var assigned in assignments)
                    {
                        workSheet.Cells[row, 1].Value = assigned.KodeDealerMPM;
                        workSheet.Cells[row, 2].Value = assigned.NamaDealer;

                        var details = _resultAppService.GetAll().Where(x => x.KodeDealerMPM == assigned.KodeDealerMPM && x.SelfRecordingId == id);

                        var workSheetDetail = package.Workbook.Worksheets.Add("Self Recording " + assigned.KodeDealerMPM);
                        workSheetDetail.Row(1).Height = 20;
                        workSheetDetail.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheetDetail.Row(1).Style.Font.Bold = true;

                        workSheetDetail.Cells[1, 1].Value = "ID MPM";
                        workSheetDetail.Cells[1, 2].Value = "Nama FLP";
                        workSheetDetail.Cells[1, 3].Value = "Kode Dealer";
                        workSheetDetail.Cells[1, 4].Value = "Nama Dealer";
                        workSheetDetail.Cells[1, 5].Value = "FLP Result";
                        workSheetDetail.Cells[1, 6].Value = "Verification Result";
                        workSheetDetail.Cells[1, 7].Value = "Verified";

                        int rowDetail = 2;

                        foreach (var item in details)
                        {
                            workSheetDetail.Cells[row, 1].Value = item.IDMPM;
                            workSheetDetail.Cells[row, 2].Value = item.NamaFLP;
                            workSheetDetail.Cells[row, 3].Value = item.KodeDealerMPM;
                            workSheetDetail.Cells[row, 4].Value = item.NamaDealerMPM;
                            workSheetDetail.Cells[row, 5].Value = item.FLPResult;
                            workSheetDetail.Cells[row, 6].Value = item.VerificationResult;
                            workSheetDetail.Cells[row, 7].Value = item.IsVerified;

                            rowDetail++;
                        }

                        workSheetDetail.Column(1).AutoFit();
                        workSheetDetail.Column(2).AutoFit();
                        workSheetDetail.Column(3).AutoFit();
                        workSheetDetail.Column(4).AutoFit();
                        workSheetDetail.Column(5).AutoFit();
                        workSheetDetail.Column(6).AutoFit();
                        workSheetDetail.Column(7).AutoFit();

                        row++;
                    }

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();

                    package.Save();
                }
            }
            catch (Exception ex)
            {

            }

            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        public ActionResult DownloadResultsDealer(string kodeDealerMPM, Guid id)
        {
            var selfRecordings = _appService.GetById(id);
            var assignment = _assignmentAppService.GetAll().SingleOrDefault(x => x.SelfRecordingId == id && x.KodeDealerMPM == kodeDealerMPM);
            var details = _resultAppService.GetAll().Where(x => x.KodeDealerMPM == kodeDealerMPM && x.SelfRecordingId == id);

            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Self Recording " + selfRecordings.Title + " Dealer " + kodeDealerMPM + " Result.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Result");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "ID MPM";
                    workSheet.Cells[1, 2].Value = "Nama FLP";
                    workSheet.Cells[1, 3].Value = "Kode Dealer";
                    workSheet.Cells[1, 4].Value = "Nama Dealer";
                    workSheet.Cells[1, 5].Value = "FLP Result";
                    workSheet.Cells[1, 6].Value = "Verification Result";
                    workSheet.Cells[1, 7].Value = "Verified";

                    int row = 2;
                    foreach (var item in details)
                    {
                        workSheet.Cells[row, 1].Value = item.IDMPM;
                        workSheet.Cells[row, 2].Value = item.NamaFLP;
                        workSheet.Cells[row, 3].Value = item.KodeDealerMPM;
                        workSheet.Cells[row, 4].Value = item.NamaDealerMPM;
                        workSheet.Cells[row, 5].Value = item.FLPResult;
                        workSheet.Cells[row, 6].Value = item.VerificationResult;
                        workSheet.Cells[row, 7].Value = item.IsVerified;

                        row++;
                    }


                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();
                    workSheet.Column(6).AutoFit();
                    workSheet.Column(7).AutoFit();

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
        #endregion

        #region Assign Dealer

        public IActionResult AssignDealer(Guid id)
        {
            var item = _appService.GetById(id);
            return View(item);
        }

        public IActionResult GetAssignedDealer(Guid id)
        {
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id).ToList();
            List<DealerVM> selectedDealer = new List<DealerVM>();

            foreach (var dealer in assignedDealer)
            {
                DealerVM assigned = new DealerVM
                {
                    id = dealer.Id,
                    nama = dealer.NamaDealer,
                    kodedealermpm = dealer.KodeDealerMPM
                };
                selectedDealer.Add(assigned);
            }

            return Json(selectedDealer.OrderBy(x => x.nama));
        }
        public IActionResult Cascading_Get_Kota([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = new DataSourceResult();
            var kota = _kotaAppService.GetAll().Select(x => new { kota = x.NamaKota }).ToList();
            result = kota.ToDataSourceResult(request);

            return Json(result);
            //var kota = _dealerAppService.GetKotaH1(karesidenan);

            //return Json(karesidenan.Select(c => new { kota = c }).ToList());
        }

        private void ModifyFilters(IEnumerable<IFilterDescriptor> filters)
        {
            if (filters.Any())
            {
                foreach (var filter in filters)
                {
                    var descriptor = filter as FilterDescriptor;
                    if (descriptor != null && descriptor.Member == "kota")
                    {
                        descriptor.Member = "kota";
                        if (descriptor.Value.ToString().Contains("KAB.") || descriptor.Value.ToString().Contains("KOTA"))
                        {
                            var sub = descriptor.Value.ToString().Substring(5);
                            descriptor.Value = sub;
                            descriptor.Operator = FilterOperator.Contains;
                        }
                    }
                    else if (filter is CompositeFilterDescriptor)
                    {
                        ModifyFilters(((CompositeFilterDescriptor)filter).FilterDescriptors);
                    }
                }
            }
        }

        public async Task<IActionResult> Get_Dealer([DataSourceRequest] DataSourceRequest request,string id, string channel, string kota)
        {
            if (kota == null || kota == "")
            {
                var task = await Task.Run(() => _dealerAppService.GetAll().Where(x => (x.Channel == channel || channel == null)).Distinct());

              
                if (!id.IsNullOrWhiteSpace())
                {
                    var assignedUser = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == Guid.Parse(id)).Select(x => x.KodeDealerMPM).ToList();
                    task = await Task.Run(() => task.Where(x => !assignedUser.Contains(x.KodeDealerMPM)));

                }
                return Json(task.ToList().OrderBy(x => x.KodeDealerMPM).ThenBy(x => x.Nama));
                //return Json(dealer);
            }
            else
            {
                var task = await Task.Run(() => _dealerAppService.GetAll().Where(x => (x.Channel == channel || channel == null) && (x.Kota.Contains(kota) || kota == null)).Distinct());
                if (!id.IsNullOrWhiteSpace())
                {
                    var assignedUser = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == Guid.Parse(id)).Select(x => x.KodeDealerMPM).ToList();
                    task = await Task.Run(() => task.Where(x => !assignedUser.Contains(x.KodeDealerMPM)));

                }
                return Json(task.ToList().OrderBy(x => x.KodeDealerMPM).ThenBy(x => x.Nama));
            }
            //internalUser = _internalUserAppService.GetAll().Where(x => (x.Channel == channel || channel == null) && (x.DealerKota == kota || kota == null) && (x.DealerName == dealer || dealer == null)).ToList();

            //return Json(internalUser.ToDataSourceResult(request));

        }

        public async Task<IActionResult> Get_Assigned_Dealer([DataSourceRequest] DataSourceRequest request, string id, string channel, string karesidenan, string kota, string dealer, string jabatan, int skip, int pageSize)
        {

            //if (limit == 0 || page == 0) {
            //    limit = 30;
            //    page = 1;
            //}
            //page = page - 1 * limit;

            var task = await Task.Run(() => _dealerAppService.GetAll());

            if (!id.IsNullOrWhiteSpace())
            {
                var assignedDealer = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == Guid.Parse(id)).Select(x => x.KodeDealerMPM).ToList();
                task = await Task.Run(() => task.Where(x => assignedDealer.Contains(x.KodeDealerMPM)));
            }

            return Json(task.ToList().OrderBy(x => x.KodeDealerMPM).ThenBy(x => x.Nama));
        }

        public async Task<IActionResult> GetCountAssignedDealer(Guid id)
        {
            var task = _dealerAppService.GetAll();
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id).Select(x => x.KodeDealerMPM).ToList();
            var count = await Task.Run(() => task.Where(x => assignedUser.Contains(x.KodeDealerMPM)).Count());
            return Json(count);
        }

        [HttpPost]
        [DisableValidation]
        public ActionResult InsertDealer(Guid id, List<DealerVM> selectedDealer)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id).ToList();
            

            if (assignedDealer.Count > 0)
            {
                List<RolePlayAssignments> deletedItem = new List<RolePlayAssignments>();

                var listInserted = selectedDealer.Where(p => !assignedDealer.Any(l => p.kodedealermpm == l.KodeDealerMPM));
               
                foreach (var inserted in listInserted)
                {
                    var dealer = _dealerAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == inserted.kodedealermpm);

                    SelfRecordingAssignments assigned = new SelfRecordingAssignments
                    {
                        Id = Guid.NewGuid(),
                        KodeDealerMPM = dealer.KodeDealerMPM,
                        NamaDealer = dealer.Nama,
                        SelfRecordingId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
                if (listInserted.Count() == 0)
                {
                    return Json(new { success = true, message = "Tidak ada dealer yang ditambahkan" });
                }
            }
            else
            {
                if (selectedDealer.Count == 0)
                {
                    return Json(new { success = true, message = "Tidak ada dealer yang ditambahkan" });
                }
                foreach (var dealer in selectedDealer)
                {
                    var assignDealer = _dealerAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == dealer.kodedealermpm);

                    SelfRecordingAssignments assigned = new SelfRecordingAssignments
                    {
                        Id = Guid.NewGuid(),
                        KodeDealerMPM = assignDealer.KodeDealerMPM,
                        NamaDealer = assignDealer.Nama,
                        SelfRecordingId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }

            return Json(new { success = true, message = "Proses berhasil" });
        }

        public ActionResult InsertAllDealer(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id).ToList();
            List<DealerVM> allDealer = _dealerAppService.GetAll().Where(x => x.DeleterUsername.IsNullOrEmpty()).Select(x => new DealerVM
            {
                kodedealermpm = x.KodeDealerMPM,
                nama = x.Nama,
            }).ToList();
            if (assignedDealer.Count > 0)
            {
                List<SelfRecordingAssignments> deletedItem = new List<SelfRecordingAssignments>();

                var listInserted = allDealer.Where(p => !assignedDealer.Any(l => p.kodedealermpm == l.KodeDealerMPM));
               
                foreach (var inserted in listInserted)
                {
                    var dealer = _dealerAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == inserted.kodedealermpm);

                    SelfRecordingAssignments assigned = new SelfRecordingAssignments
                    {
                        Id = Guid.NewGuid(),
                        KodeDealerMPM = dealer.KodeDealerMPM,
                        NamaDealer = dealer.Nama,
                        SelfRecordingId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
                if (listInserted.Count() == 0)
                {
                    return Json(new { success = true, message = "Tidak ada dealer yang ditambahkan" });
                }
            }
            else
            {
                if (allDealer.Count == 0)
                {
                    return Json(new { success = true, message = "Tidak ada dealer yang ditambahkan" });
                }
                foreach (var dealer in allDealer)
                {
                    var assignDealer = _dealerAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == dealer.kodedealermpm);

                    SelfRecordingAssignments assigned = new SelfRecordingAssignments
                    {
                        Id = Guid.NewGuid(),
                        KodeDealerMPM = assignDealer.KodeDealerMPM,
                        NamaDealer = assignDealer.Nama,
                        SelfRecordingId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }

            return Json(new { success = true, message = "Proses berhasil" });
        }



        [DisableValidation]
        public ActionResult RemoveDealer(Guid id, List<DealerVM> selectedDealer)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id).ToList();

            if (assignedDealer.Count > 0)
            {
                List<RolePlayAssignments> deletedItem = new List<RolePlayAssignments>();

                //var listInserted = selectedDealer.Where(p => !assignedDealer.Any(l => p.id == l.Id));
                var listDeleted = assignedDealer.Where(p => selectedDealer.Any(l => p.KodeDealerMPM == l.kodedealermpm));

                foreach (var deleted in listDeleted)
                {
                    _assignmentAppService.Delete(deleted.Id);
                }
             
                if (listDeleted.Count() == 0)
                {
                    return Json(new { success = true, message = "Tidak ada dealer yang dihapus" });
                }
            }
            else
            {
                if (selectedDealer.Count == 0)
                {
                    return Json(new { success = true, message = "Tidak ada dealer yang dihapus" });
                }
               
            }

            return Json(new { success = true, message = "Proses berhasil" });
        }

        public ActionResult RemoveAllDealer(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id).ToList();
            List<DealerVM> allDealer = _dealerAppService.GetAll().Where(x => x.DeleterUsername.IsNullOrEmpty()).Select(x => new DealerVM
            {
                //id = Guid.Parse(x.Id),
                kodedealermpm = x.KodeDealerMPM,
                nama = x.Nama,
            }).ToList();
            if (assignedDealer.Count > 0)
            {
                List<SelfRecordingAssignments> deletedItem = new List<SelfRecordingAssignments>();

                //var listInserted = selectedDealer.Where(p => !assignedDealer.Any(l => p.id == l.Id));
                var listDeleted = assignedDealer.Where(p => allDealer.Any(l => p.KodeDealerMPM == l.kodedealermpm));

                foreach (var deleted in listDeleted)
                {
                    _assignmentAppService.Delete(deleted.Id);
                }
             
                if (listDeleted.Count() == 0)
                {
                    return Json(new { success = true, message = "Tidak ada dealer yang dihapus" });
                }
            }
            else
            {
                if (allDealer.Count == 0)
                {
                    return Json(new { success = true, message = "Tidak ada dealer yang dihapus" });
                }
              
            }

            return Json(new { success = true, message = "Proses berhasil" });
        }


        #endregion

        #region Dealer
        public IActionResult Dealer(Guid id)
        {
            var model = _appService.GetById(id);
            return View(model);
        }

        public IActionResult Grid_Dealer_Read([DataSourceRequest] DataSourceRequest request, RolePlays roleplay)
        {
            DataSourceResult result = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == roleplay.Id).OrderBy(x => x.KodeDealerMPM).ToList().ToDataSourceResult(request);

            return Json(result);
        }

        #endregion
    }
}