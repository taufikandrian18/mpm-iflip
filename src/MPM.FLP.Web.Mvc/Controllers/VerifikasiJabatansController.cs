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
using MPM.FLP.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using MPM.FLP.Authorization.Users;
using Abp.Runtime.Security;
using MPM.FLP.Authorization.Roles;
using Abp.Runtime.Session;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using MPM.FLP.Web.Models.FLPMPM;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.DataValidation;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class VerifikasiJabatansController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly VerifikasiJabatanAppService _appService;
        private readonly JabatanAppService _jabatanAppService;
        private readonly VerifikasiJabatanQuestionAppService _questionAppService;
        private readonly HomeworkQuizHistoryAppService _historyAppService;
        private readonly HomeworkQuizAnswerAppService _historyAnswerAppService;
        private readonly KotaAppService _kotaAppService;
        private readonly DealerAppService _dealerAppService;
        private readonly InternalUserAppService _internalUserAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public VerifikasiJabatansController(VerifikasiJabatanAppService appService, JabatanAppService jabatanAppService, UserManager userManager, VerifikasiJabatanQuestionAppService questionAppService, HomeworkQuizHistoryAppService historyAppService, HomeworkQuizAnswerAppService historyAnswerAppService, KotaAppService kotaAppService, DealerAppService dealerAppService, InternalUserAppService internalUserAppService, IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _jabatanAppService = jabatanAppService;
            _questionAppService = questionAppService;
            _userManager = userManager;
            _historyAppService = historyAppService;
            _kotaAppService = kotaAppService;
            _dealerAppService = dealerAppService;
            _internalUserAppService = internalUserAppService;
            _hostingEnvironment = hostingEnvironment;
            _historyAnswerAppService = historyAnswerAppService;
        }

        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            
            return View();
        }

        public IActionResult Get_IdGroupJabatan([DataSourceRequest]DataSourceRequest request)
        {  
            DataSourceResult result = _jabatanAppService.GetAll().Where(x=> x.Channel == "H1" && !string.IsNullOrEmpty(x.IDGroupJabatan)).Select(c => new { idgroupjabatan = c.IDGroupJabatan }).Distinct().ToList().ToDataSourceResult(request);

            return Json(result);
            //return Json(karesidenan.Select(c => new { karesidenan = c }).ToList());
        }

        public IActionResult Create(VerifikasiJabatans model)
        {
            //var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
            //var roles = _userManager.GetRolesAsync(user).Result.ToList();

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(VerifikasiJabatans model, string submit)
        {
            if (model != null)
            {

                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.DeleterUsername = "";


                _appService.Create(model);
            }
            //return RedirectToAction("Index");
            return Json(new { success = true, message = model.Id });
        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetById(id);

            return View(item);
        }


        [HttpPost]
        public IActionResult Edit(VerifikasiJabatans model, string submit)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);

                _appService.Update(model);
            }
            return Json(new { success = true });
        }

        public IActionResult EditAttachment(Guid id)
        {
            var item = _appService.GetById(id);


            return View(item);
            //return View();
        }
        
        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {

            DataSourceResult result = _appService.GetAll().Where(x =>  string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x=>x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, HomeworkQuizzes item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id,this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        #region Histories
        public IActionResult Histories()
        {
            TempData["alert"] = "";
            TempData["success"] = "";

            return View();
        }
        public IActionResult Grid_Histories_Read([DataSourceRequest]DataSourceRequest request)
        {

            DataSourceResult result = _historyAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }
        #endregion

        #region Homework Question
        //Homework Question
        public IActionResult Questions(Guid id)
        {
            var item = _appService.GetById(id);
            return View(item);
        }
        public IActionResult BackToQuestions(Guid id)
        {
            var model = _questionAppService.GetById(id);
            var item = _appService.GetById(model.VerfikasiJabatanId);
            return View(item);
        }
        public IActionResult EditQuestion(Guid id)
        {
            var item = _questionAppService.GetById(id);
            var model = new QuestionsVM();
            model.Id = item.Id;
            model.CreationTime = item.CreationTime;
            model.CreatorUsername = item.CreatorUsername;
            model.Question = item.Question;
            model.OptionA = item.OptionA;
            model.OptionB = item.OptionB;
            model.OptionC = item.OptionC;
            model.OptionD = item.OptionD;
            model.OptionE = item.OptionE;
            model.CorrectOption = item.CorrectOption;
            model.ParentId = item.VerfikasiJabatanId;
            //model.veri = item.HomeworkQuizId;
            model.ImageUrl = item.ImageUrl;

            return View(model);
        }

        public IActionResult CreateQuestion(Guid id)
        {
            QuestionsVM item = new QuestionsVM();
            item.ParentId = id;
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> CreateQuestion(QuestionsVM item, string submit, IEnumerable<IFormFile> images)
        {
            VerifikasiJabatanQuestions model = new VerifikasiJabatanQuestions();
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.DeleterUsername = "";
                model.Question = item.Question;
                model.OptionA = item.OptionA;
                model.OptionB = item.OptionB;
                model.OptionC = item.OptionC;
                model.OptionD = item.OptionD;
                model.OptionE = item.OptionE;
                model.CorrectOption = item.CorrectOption;
                model.VerfikasiJabatanId = item.ParentId;

                if (images.Count() > 0)
                {
                    model.ImageUrl = await new AzureController().InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "verifikasijabatan");
                }
                else
                {
                    model.ImageUrl = "";
                }

                _questionAppService.Create(model);
            }
            //return RedirectToAction("Index");
            return Json(new { success = true, message = item.ParentId });
        }

        [HttpPost]
        public async Task<IActionResult> EditQuestion(QuestionsVM item, string submit, IEnumerable<IFormFile> images)
        {
            VerifikasiJabatanQuestions model = _questionAppService.GetById(item.Id);
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.Question = item.Question;
                model.OptionA = item.OptionA;
                model.OptionB = item.OptionB;
                model.OptionC = item.OptionC;
                model.OptionD = item.OptionD;
                model.OptionE = item.OptionE;
                model.CorrectOption = item.CorrectOption;

                if (images.Count() > 0)
                {
                    model.ImageUrl = await new AzureController().InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "verifikasijabatan");
                }
                else
                {
                    if (item.ImageUrl == null) 
                    {
                        model.ImageUrl = "";
                    }
                    else
                    {
                        model.ImageUrl = item.ImageUrl;
                    }
                    
                }

                _questionAppService.Update(model);
            }
            //return RedirectToAction("Index");
            return Json(new { success = true, message = item.ParentId });
        }

        public IActionResult GridQuestion_Read([DataSourceRequest]DataSourceRequest request, HomeworkQuizzes model)
        {

            DataSourceResult result = _questionAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.VerfikasiJabatanId == model.Id).OrderBy(x => x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult GridQuestion_Destroy([DataSourceRequest]DataSourceRequest request, HomeworkQuizQuestions item)
        {
            if (ModelState.IsValid)
            {
                _questionAppService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DownloadTemplate()
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Template Pertanyan Verifikasi Jabatan.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Quiz");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Pertanyaan";
                    workSheet.Cells[1, 2].Value = "Opsi A";
                    workSheet.Cells[1, 3].Value = "Opsi B";
                    workSheet.Cells[1, 4].Value = "Opsi C";
                    workSheet.Cells[1, 5].Value = "Opsi D";
                    workSheet.Cells[1, 6].Value = "Opsi E";
                    workSheet.Cells[1, 7].Value = "Jawaban";

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();
                    workSheet.Column(6).AutoFit();
                    workSheet.Column(7).AutoFit();
                    workSheet.Column(8).AutoFit();

                    var validation = workSheet.DataValidations.AddListValidation("G2:G1000");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    validation.ErrorTitle = "An invalid value was entered";
                    validation.Error = "Select a value from the list";
                    validation.Formula.Values.AddRange(new List<string> { "A", "B", "C", "D", "E"});
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
        public async Task<IActionResult> ImportExcel(VerifikasiJabatans model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var file = files.FirstOrDefault();
                    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewBag.message = "Format file hanya mendukung .xlsx";
                        return Redirect("~/VerifikasiJabatans/ImportExcel/" + model.Id);
                    }

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Quiz"];
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
                                var question = worksheet.Cells[row, 1].Value.ToString().Trim();
                                var opsiA = worksheet.Cells[row, 2].Value.ToString().Trim();
                                var opsiB = worksheet.Cells[row, 3].Value.ToString().Trim();
                                var opsiC = worksheet.Cells[row, 4].Value.ToString().Trim();
                                var opsiD = worksheet.Cells[row, 5].Value.ToString().Trim();
                                var opsiE = worksheet.Cells[row, 6].Value.ToString().Trim();
                                var answer = worksheet.Cells[row, 7].Value.ToString().Trim();
                                

                                if (question == null)
                                {
                                    TempData["alert"] = "Data pertanyaan pada baris " + row + " masih kosong";
                                    TempData["success"] = "";
                                    return RedirectToAction("ImportExcel/" + model.Id);
                                }
                                VerifikasiJabatanQuestions quizQuestions = new VerifikasiJabatanQuestions
                                {
                                    Id = Guid.NewGuid(),
                                    ImageUrl = "",
                                    Question = question,
                                    OptionA = opsiA,
                                    OptionB = opsiB,
                                    OptionC = opsiC,
                                    OptionD = opsiD,
                                    OptionE = opsiE,
                                    CorrectOption = answer,
                                    VerfikasiJabatanId = model.Id,
                                    CreationTime = DateTime.Now,
                                    CreatorUsername = this.User.Identity.Name,
                                    LastModifierUsername = this.User.Identity.Name,
                                    LastModificationTime = DateTime.Now,
                                    DeleterUsername = ""
                                };
                                _questionAppService.Create(quizQuestions);
                            }
                        }
                    }
                }

                TempData["success"] = "Berhasil menambah data";
                TempData["alert"] = "";
            }
            return Redirect("~/VerifikasiJabatans/Questions/" + model.Id);
        }

        #endregion

    }
}