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
using Kendo.Mvc;
using Abp.Extensions;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class LiveQuizController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly LiveQuizAppService _appService;
        private readonly LiveQuizQuestionAppService _questionAppService;
        private readonly LiveQuizAssignmentAppService _assignmentAppService;
        private readonly LiveQuizHistoryAppService _historyAppService;
        private readonly LiveQuizAnswerAppService _historyAnswerAppService;
        private readonly KotaAppService _kotaAppService;
        private readonly DealerAppService _dealerAppService;
        private readonly InternalUserAppService _internalUserAppService;
        private readonly JabatanAppService _jabatanAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public LiveQuizController(LiveQuizAppService appService, LiveQuizQuestionAppService questionAppService, UserManager userManager, LiveQuizAssignmentAppService assignmentAppService, LiveQuizHistoryAppService historyAppService, LiveQuizAnswerAppService historyAnswerAppService, KotaAppService kotaAppService, DealerAppService dealerAppService, InternalUserAppService internalUserAppService, JabatanAppService jabatanAppService, IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _questionAppService = questionAppService;
            _assignmentAppService = assignmentAppService;
            _userManager = userManager;
            _historyAppService = historyAppService;
            _kotaAppService = kotaAppService;
            _dealerAppService = dealerAppService;
            _internalUserAppService = internalUserAppService;
            _hostingEnvironment = hostingEnvironment;
            _historyAnswerAppService = historyAnswerAppService;
            _jabatanAppService = jabatanAppService;
        }

        #region Main
        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";

            return View();
        }

        public IActionResult Create(LiveQuizzes model)
        {
            //var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
            //var roles = _userManager.GetRolesAsync(user).Result.ToList();

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(LiveQuizzes model, string submit, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {

                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.DeleterUsername = "";
                model.FeaturedImageUrl = "";

                foreach (var image in images)
                {
                    AzureController azureController = new AzureController();
                    model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(image, model.Id.ToString(), "IMG", "livequiz");
                }

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
        public async Task<IActionResult> Edit(LiveQuizzes model, string submit, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);


                foreach (var image in images)
                {
                    AzureController azureController = new AzureController();
                    model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(image, model.Id.ToString(), "IMG", "livequiz");
                }

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

        public IActionResult Grid_Read([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest] DataSourceRequest request, LiveQuizzes item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Live Question
        //Homework Question
        public IActionResult LiveQuestion(Guid id)
        {
            var item = _appService.GetById(id);
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
            model.ParentId = item.LiveQuizId;
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
            LiveQuizQuestions model = new LiveQuizQuestions();
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
                model.LiveQuizId = item.ParentId;

                if (images.Count() > 0)
                {
                    model.ImageUrl = await new AzureController().InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "livequizquestion");
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
            LiveQuizQuestions model = _questionAppService.GetById(item.Id);
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
                model.LiveQuizId = item.ParentId;

                if (images.Count() > 0)
                {
                    model.ImageUrl = await new AzureController().InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "livequizquestion");
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

        public IActionResult GridQuestion_Read([DataSourceRequest] DataSourceRequest request, LiveQuizzes model)
        {

            DataSourceResult result = _questionAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.LiveQuizId == model.Id).OrderBy(x => x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult GridQuestion_Destroy([DataSourceRequest] DataSourceRequest request, LiveQuizQuestions item)
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
            string excelName = "Live Quiz Question Template.xlsx";

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
                    validation.Formula.Values.AddRange(new List<string> { "A", "B", "C", "D", "E" });
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
        public async Task<IActionResult> ImportExcel(LiveQuizzes model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var file = files.FirstOrDefault();
                    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewBag.message = "Format file hanya mendukung .xlsx";
                        return Redirect("~/LiveQuiz/ImportExcel/" + model.Id);
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
                            var isEmpty = false;
                            //TempData["alert"] = "Bahaya";
                            //return RedirectToAction("VarianHarga/" + model.Id);
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var question = worksheet.Cells[row, 1].Value?.ToString().Trim();
                                var opsiA = worksheet.Cells[row, 2].Value?.ToString().Trim();
                                var opsiB = worksheet.Cells[row, 3].Value?.ToString().Trim();
                                var opsiC = worksheet.Cells[row, 4].Value?.ToString().Trim();
                                var opsiD = worksheet.Cells[row, 5].Value?.ToString().Trim();
                                var opsiE = worksheet.Cells[row, 6].Value?.ToString().Trim();
                                var answer = worksheet.Cells[row, 7].Value?.ToString().Trim();


                                if (question == null || opsiA == null || opsiB == null || opsiC == null || opsiD == null || opsiE == null || answer == null)
                                {
                                    TempData["alert"] = "Ada data pada baris " + row + " yang masih kosong";
                                    TempData["success"] = "";
                                    return RedirectToAction("ImportExcel/" + model.Id);
                                }
                                LiveQuizQuestions quizQuestions = new LiveQuizQuestions
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
                                    LiveQuizId = model.Id,
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
            return Redirect("~/LiveQuiz/LiveQuestion/" + model.Id);
        }

        #endregion

        #region Assign User

        public IActionResult AssignUser(Guid id)
        {
            var item = _appService.GetById(id);
            return View(item);
        }

        public IActionResult GetAssignedUser(Guid id)
        {
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).ToList();
            List<InternalUsersVM> selectedUser = new List<InternalUsersVM>();

            foreach (var user in assignedUser)
            {
                InternalUsersVM assigned = new InternalUsersVM
                {
                    Idmpm = user.IDMPM,
                    Nama = _internalUserAppService.GetAll().SingleOrDefault(x => x.IDMPM == user.IDMPM).Nama,
                    Jabatan = _internalUserAppService.GetAll().SingleOrDefault(x => x.IDMPM == user.IDMPM).Jabatan,
                    DealerKota = _internalUserAppService.GetAll().SingleOrDefault(x => x.IDMPM == user.IDMPM).DealerKota,
                    DealerName = _internalUserAppService.GetAll().SingleOrDefault(x => x.IDMPM == user.IDMPM).DealerName,
                    Channel = _internalUserAppService.GetAll().SingleOrDefault(x => x.IDMPM == user.IDMPM).Channel,
                    IDGroupJabatan = _internalUserAppService.GetAll().SingleOrDefault(x => x.IDMPM == user.IDMPM).IDGroupJabatan
                };
                selectedUser.Add(assigned);
            }

            return Json(selectedUser.OrderBy(x => x.IDGroupJabatan).ThenBy(x => x.Idmpm));
        }

        public IActionResult Cascading_Get_Jabatan([DataSourceRequest] DataSourceRequest request, string channel)
        {
            DataSourceResult result = new DataSourceResult();
            var jabatan = _jabatanAppService.GetAll().Where(x => x.Channel == channel || channel == null).Select(x => new { idgroupjabatan = x.IDGroupJabatan }).Distinct().ToList();
            result = jabatan.ToDataSourceResult(request);

            return Json(result);
            //var kota = _dealerAppService.GetKotaH1(karesidenan);

            //return Json(karesidenan.Select(c => new { kota = c }).ToList());
        }

        public IActionResult Cascading_Get_Kota([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = new DataSourceResult();
            var kota = _kotaAppService.GetAll().Select(x => new { kota = x.NamaKota }).ToList();
            kota.Add(new { kota = " Semua Kota" });
            result = kota.OrderBy(x => x.kota).ToDataSourceResult(request);

            return Json(result);
            //var kota = _dealerAppService.GetKotaH1(karesidenan);

            //return Json(karesidenan.Select(c => new { kota = c }).ToList());
        }

        public IActionResult Cascading_Get_Dealer([DataSourceRequest] DataSourceRequest request, string kota)
        {
            ModifyFilters(request.Filters);

            DataSourceResult result = new DataSourceResult();
            if (kota.IsNullOrWhiteSpace() || kota == " Semua Kota")
            {
                DataSourceRequest req = new DataSourceRequest();
                var dealer = _dealerAppService.GetAll().Select(x => new { kota = x.Kota, dealer = x.Nama, kode = x.KodeDealerMPM }).Distinct().ToList();
                result = dealer.OrderBy(x => x.kode).ToDataSourceResult(req);
            }
            else
            {
                var dealer = _dealerAppService.GetAll().Where(x => kota.Contains(x.Kota) && x.Kota != "").Select(x => new { kota = x.Kota, dealer = x.Nama, kode = x.KodeDealerMPM }).Distinct().ToList();
                result = dealer.OrderBy(x => x.kode).ToDataSourceResult(request);
            }


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

        public async Task<IActionResult> Get_User([DataSourceRequest] DataSourceRequest request, string id,  string channel, string karesidenan, string kota, string dealer, string jabatan, int limit, int page)
        {
            if (limit == 0 || page == 0)
            {
                limit = 30;
                page = 1;
            }
            page = page - 1 * limit;

       

            //var task = await Task.Run(() => _internalUserAppService.GetAll().Where(x => (x.Channel == channel || channel == null) && (kota.Contains(x.DealerKota) || kota == null) && (dealer.Contains(x.DealerName) || dealer == null) && (x.IDGroupJabatan == jabatan || jabatan == null)).ToList());
            //var task = await Task.Run(() => _internalUserAppService.GetAll().Where(x => (x.Channel == channel || channel == null) && (x.DealerKota.Contains(kota) || kota == null) && (dealer.Contains(x.DealerName) || dealer == null) && (x.IDGroupJabatan == jabatan || jabatan == null)).ToList());
            //internalUser = _internalUserAppService.GetAll().Where(x => (x.Channel == channel || channel == null) && (x.DealerKota == kota || kota == null) && (x.DealerName == dealer || dealer == null)).ToList();
            var task = await Task.Run(() => _internalUserAppService.GetAll());
            if (!channel.IsNullOrWhiteSpace()) {
                task = await Task.Run(() => task.Where(x => x.Channel == channel));
            }
            if (!jabatan.IsNullOrWhiteSpace()) {
                task = await Task.Run(() => task.Where(x => x.IDGroupJabatan == jabatan));
            }
            if (!kota.IsNullOrWhiteSpace() && kota != " Semua Kota")
            {
                task = await Task.Run(() => task.Where(x => x.DealerKota.Contains(kota)));
            }
            if (!dealer.IsNullOrWhiteSpace()) {
                task = await Task.Run(() => task.Where(x => x.DealerName.Contains(dealer)));
            }
            if (!id.IsNullOrWhiteSpace())
            {
                var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == Guid.Parse(id)).Select(x => x.IDMPM).ToList();
                task = await Task.Run(() => task.Where(x => !assignedUser.Contains(x.IDMPM)));

            }
            //return Json(internalUser.ToDataSourceResult(request));
            return Json(task.ToList().OrderBy(x => x.IDGroupJabatan).ThenBy(x => x.IDMPM).Skip(page).Take(limit));
        }

        public async Task<IActionResult> GetCountAssignedUser(Guid id)
        {
            var task = _internalUserAppService.GetAll();
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).Select(x => x.IDMPM).ToList();
            var count = await Task.Run(() => task.Where(x => assignedUser.Contains(x.IDMPM)).Count());
            return Json(count);
        }

        public async Task<IActionResult> Get_Assigned_User([DataSourceRequest] DataSourceRequest request, string id, string channel, string karesidenan, string kota, string dealer, string jabatan, int skip, int pageSize)
        {

            //if (limit == 0 || page == 0) {
            //    limit = 30;
            //    page = 1;
            //}
            //page = page - 1 * limit;


            var task = await Task.Run(() => _internalUserAppService.GetAll());

            if (!id.IsNullOrWhiteSpace())
            {
                var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == Guid.Parse(id)).Select(x => x.IDMPM).ToList();
                task = await Task.Run(() => task.Where(x => assignedUser.Contains(x.IDMPM)));
            }

            return Json(task.ToList().OrderBy(x => x.IDGroupJabatan).ThenBy(x => x.IDMPM));
        }

        [HttpPost]
        public ActionResult InsertUser(Guid id, List<InternalUsersVM> selectedUser)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).ToList();

            if (assignedUser.Count > 0)
            {
                List<LiveQuizAssignments> deletedItem = new List<LiveQuizAssignments>();
                var listInserted = selectedUser.Where(p => !assignedUser.Any(l => p.Idmpm == l.IDMPM));
                //var listDeleted = assignedUser.Where(p => !selectedUser.Any(l => p.IDMPM == l.Idmpm));

                //foreach(var deleted in listDeleted)
                //{
                //    _assignmentAppService.Delete(deleted.Id);
                //}
                foreach (var inserted in listInserted)
                {
                    LiveQuizAssignments assigned = new LiveQuizAssignments
                    {
                        Id = Guid.NewGuid(),
                        IDMPM = inserted.Idmpm,
                        LiveQuizId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
                if( listInserted.Count() == 0)
                {
                    return Json(new { success = true, message = "Tidak ada user yang ditambahkan" });
                }
            }
            else
            {
                if(selectedUser.Count == 0)
                {
                    return Json(new { success = true, message = "Tidak ada user yang ditambahkan" });
                }
                foreach(var user in selectedUser)
                {
                    LiveQuizAssignments assigned = new LiveQuizAssignments
                    {
                        Id = Guid.NewGuid(),
                        IDMPM = user.Idmpm,
                        LiveQuizId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }
            
            return Json(new { success = true, message = "Proses berhasil" });
        }

        [HttpPost]
        public ActionResult InsertAllUser(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).ToList();
            List<InternalUsersVM> allUser = _internalUserAppService.GetAll().Where(x => x.DeleterUsername.IsNullOrEmpty()).Select(x => new InternalUsersVM
            {
                Channel = x.Channel,
                Dealer = x.DealerName,
                DealerKota = x.DealerKota,
                DealerName = x.DealerName,
                IDGroupJabatan = x.IDGroupJabatan,
                Idmpm = x.IDMPM,
                Jabatan = x.Jabatan,
                Nama = x.Nama,
                Kota = x.DealerKota
            }).ToList();
            if (assignedUser.Count > 0)
            {
                List<HomeworkQuizAssignments> deletedItem = new List<HomeworkQuizAssignments>();
                var listInserted = allUser.Where(p => !assignedUser.Any(l => p.Idmpm == l.IDMPM));
                //var listDeleted = assignedUser.Where(p => !selectedUser.Any(l => p.IDMPM == l.Idmpm));

                //foreach(var deleted in listDeleted)
                //{
                //    _assignmentAppService.Delete(deleted.Id);
                //}
                foreach (var inserted in listInserted)
                {
                    LiveQuizAssignments assigned = new LiveQuizAssignments
                    {
                        Id = Guid.NewGuid(),
                        IDMPM = inserted.Idmpm,
                        LiveQuizId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
                if (listInserted.Count() == 0)
                {
                    return Json(new { success = true, message = "Tidak ada user yang ditambahkan" });
                }
            }
            else
            {
                if (allUser.Count == 0)
                {
                    return Json(new { success = true, message = "Tidak ada user yang ditambahkan" });
                }
                foreach (var user in allUser)
                {
                    LiveQuizAssignments assigned = new LiveQuizAssignments
                    {
                        Id = Guid.NewGuid(),
                        IDMPM = user.Idmpm,
                        LiveQuizId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }

            return Json(new { success = true, message = "Proses berhasil" });
        }

        [HttpPost]
        public ActionResult RemoveUser(Guid id, List<InternalUsersVM> selectedUser)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).ToList();

            if (assignedUser.Count > 0)
            {
                List<LiveQuizAssignments> deletedItem = new List<LiveQuizAssignments>();
                //var listInserted = selectedUser.Where(p => !assignedUser.Any(l => p.Idmpm == l.IDMPM));
                var listDeleted = assignedUser.Where(p => selectedUser.Any(l => p.IDMPM == l.Idmpm));

                foreach (var deleted in listDeleted)
                {
                    _assignmentAppService.Delete(deleted.Id);
                }
                //foreach (var inserted in listInserted)
                //{
                //    LiveQuizAssignments assigned = new LiveQuizAssignments
                //    {
                //        Id = Guid.NewGuid(),
                //        IDMPM = inserted.Idmpm,
                //        LiveQuizId = id
                //    };

                //    _assignmentAppService.Create(assigned);
                //}
                if (listDeleted.Count() == 0)
                {
                    return Json(new { success = true, message = "Tidak ada user yang dihapus" });
                }
            }
            else
            {
                if (selectedUser.Count == 0)
                {
                    return Json(new { success = true, message = "Tidak ada user yang dihapus" });
                }
                //foreach (var user in selectedUser)
                //{
                //    LiveQuizAssignments assigned = new LiveQuizAssignments
                //    {
                //        Id = Guid.NewGuid(),
                //        IDMPM = user.Idmpm,
                //        LiveQuizId = id
                //    };

                //    _assignmentAppService.Create(assigned);
                //}
            }

            return Json(new { success = true, message = "Proses berhasil" });
        }

        [HttpPost]
        public ActionResult RemoveAllUser(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).ToList();
            List<InternalUsersVM> allUser = _internalUserAppService.GetAll().Where(x => x.DeleterUsername.IsNullOrEmpty()).Select(x => new InternalUsersVM
            {
                Channel = x.Channel,
                Dealer = x.DealerName,
                DealerKota = x.DealerKota,
                DealerName = x.DealerName,
                IDGroupJabatan = x.IDGroupJabatan,
                Idmpm = x.IDMPM,
                Jabatan = x.Jabatan,
                Nama = x.Nama,
                Kota = x.DealerKota
            }).ToList();
            if (assignedUser.Count > 0)
            {
                List<LiveQuizAssignments> deletedItem = new List<LiveQuizAssignments>();
                //var listInserted = selectedUser.Where(p => !assignedUser.Any(l => p.Idmpm == l.IDMPM));
                var listDeleted = assignedUser.Where(p => allUser.Any(l => p.IDMPM == l.Idmpm));

                foreach (var deleted in listDeleted)
                {
                    _assignmentAppService.Delete(deleted.Id);
                }
                //foreach (var inserted in listInserted)
                //{
                //    LiveQuizAssignments assigned = new LiveQuizAssignments
                //    {
                //        Id = Guid.NewGuid(),
                //        IDMPM = inserted.Idmpm,
                //        LiveQuizId = id
                //    };

                //    _assignmentAppService.Create(assigned);
                //}
                if (listDeleted.Count() == 0)
                {
                    return Json(new { success = true, message = "Tidak ada user yang dihapus" });
                }
            }
            else
            {
                if (allUser.Count == 0)
                {
                    return Json(new { success = true, message = "Tidak ada user yang dihapus" });
                }
                //foreach (var user in allUser)
                //{
                //    LiveQuizAssignments assigned = new LiveQuizAssignments
                //    {
                //        Id = Guid.NewGuid(),
                //        IDMPM = user.Idmpm,
                //        LiveQuizId = id
                //    };

                //    _assignmentAppService.Create(assigned);
                //}
            }

            return Json(new { success = true, message = "Proses berhasil" });
        }

        #endregion

        #region Score

        public IActionResult LiveQuizScore(Guid id)
        {
            var item = _appService.GetById(id);
            return View(item);
        }

        public IActionResult Grid_Read_LiveQuizScore([DataSourceRequest]DataSourceRequest request, LiveQuizzes model)
        {
            DataSourceResult result = _historyAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.LiveQuizId == model.Id).OrderByDescending(x => x.Score).ToDataSourceResult(request);

            return Json(result);
        }

        public ActionResult DownloadScore(Guid id)
        {
            var model = _appService.GetById(id);
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Homework Quiz " + model.Title +".xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Hasil Quiz");
                    var workSheet2 = package.Workbook.Worksheets.Add("Detail Jawaban Quiz");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet2.Row(1).Height = 20;
                    workSheet2.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet2.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Id MPM";
                    workSheet.Cells[1, 2].Value = "Nama";
                    workSheet.Cells[1, 3].Value = "Jabatan";
                    workSheet.Cells[1, 4].Value = "Kota";
                    workSheet.Cells[1, 5].Value = "Dealer";
                    workSheet.Cells[1, 6].Value = "Jawaban Benar";
                    workSheet.Cells[1, 7].Value = "Jawaban Salah";
                    workSheet.Cells[1, 8].Value = "Skor";

                    workSheet2.Cells[1, 1].Value = "Id MPM";
                    workSheet2.Cells[1, 2].Value = "Pertanyaan";
                    workSheet2.Cells[1, 3].Value = "Jawaban";

                    var hwQuizScore = _historyAppService.GetAll().Where(x=>x.LiveQuizId == id && string.IsNullOrEmpty(x.DeleterUsername)).ToList();
                    int detailRow = 2;
                    int row = 2;
                    foreach (var score in hwQuizScore)
                    {
                        workSheet.Cells[row, 1].Value = score.IDMPM;
                        workSheet.Cells[row, 2].Value = score.Name;
                        workSheet.Cells[row, 3].Value = score.Jabatan;
                        workSheet.Cells[row, 4].Value = score.Kota;
                        workSheet.Cells[row, 5].Value = score.Dealer;
                        workSheet.Cells[row, 6].Value = score.CorrectAnswer;
                        workSheet.Cells[row, 7].Value = score.WrongAnswer;
                        workSheet.Cells[row, 8].Value = score.Score;

                        //var details = _historyAnswerAppService.GetByLiveQuizHistory(score.Id);
                       
                        foreach (var detail in score.LiveQuizAnswers)
                        {
                            workSheet2.Cells[detailRow, 1].Value = score.IDMPM;
                            workSheet2.Cells[detailRow, 2].Value = detail.Question;
                            workSheet2.Cells[detailRow, 3].Value = detail.Answer;
                            detailRow++;
                        }

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

                    //var validation = workSheet.DataValidations.AddListValidation("B2:B200");
                    //validation.ShowErrorMessage = true;
                    //validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    //validation.ErrorTitle = "An invalid value was entered";
                    //validation.Error = "Select a value from the list";

                    


                    workSheet2.Column(1).AutoFit();
                    workSheet2.Column(2).AutoFit();
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
    }
}