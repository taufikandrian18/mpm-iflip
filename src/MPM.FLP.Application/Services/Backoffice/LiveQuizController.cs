using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using MPM.FLP.Authorization.Users;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;

namespace MPM.FLP.Services.Backoffice
{
    public class LiveQuizController : FLPAppServiceBase, ILiveQuizController
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
        [HttpGet("/api/services/app/backoffice/LiveQuiz/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.CreatorUsername.Contains(request.Query) || x.Title.Contains(request.Query) || x.TotalQuestion.ToString() == request.Query);
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/LiveQuiz/getByID")]
        public LiveQuizzes GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }


        [HttpPost("/api/services/app/backoffice/LiveQuiz/create")]
        public async Task<LiveQuizzes> Create([FromForm]LiveQuizzes model, [FromForm]IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
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
            
            return model;
        }


        [HttpPut("/api/services/app/backoffice/LiveQuiz/update")]
        public async Task<LiveQuizzes> Edit(LiveQuizzes model, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);


                foreach (var image in images)
                {
                    AzureController azureController = new AzureController();
                    model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(image, model.Id.ToString(), "IMG", "livequiz");
                }

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/LiveQuiz/destroy")]
        public String Grid_Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        #endregion

        #region Question
        [HttpGet("/api/services/app/backoffice/LiveQuiz/getAllQuestion")]
        public BaseResponse GetAllQuestion([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _questionAppService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => 
                    x.CreatorUsername.Contains(request.Query) || 
                    x.Question.Contains(request.Query) || 
                    x.LiveQuizId.ToString() == request.Query
                );
            }

            if(!string.IsNullOrEmpty(request.ParentId)){
                query = query.Where(x=> x.LiveQuizId.ToString() == request.ParentId);
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/LiveQuiz/getQuestionByID")]
        public LiveQuizQuestions GetQuestionByID(Guid guid)
        {
            return _questionAppService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/LiveQuiz/createQuestion")]
        public async Task<LiveQuizQuestions> CreateQuestion([FromForm]QuestionsVM item, [FromForm]IEnumerable<IFormFile> images)
        {
            LiveQuizQuestions model = new LiveQuizQuestions();
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
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
            
            return model;
        }

        [HttpPut("/api/services/app/backoffice/LiveQuiz/editQuestion")]
        public async Task<LiveQuizQuestions> EditQuestion([FromForm]QuestionsVM item, [FromForm]IEnumerable<IFormFile> images)
        {
            LiveQuizQuestions model = _questionAppService.GetById(item.Id);
            if (model != null)
            {
                model.LastModifierUsername = "admin";
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
            
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/LiveQuiz/destroyQuestion")]
        public String DestroyQuestion(Guid guid)
        {
            _questionAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/LiveQuiz/downloadTemplate")]
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
                    validation.Formula.Values.Add("A");
                    validation.Formula.Values.Add("B");
                    validation.Formula.Values.Add("C");
                    validation.Formula.Values.Add("D");
                    validation.Formula.Values.Add("E");
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

       

        [HttpPost("/api/services/app/backoffice/LiveQuiz/importExcel")]
        public async Task<String> ImportExcel([FromForm]LiveQuizzes model, [FromForm]IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var file = files.FirstOrDefault();
                    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        return "Format file hanya mendukung .xlsx";
                    }

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Quiz"];
                            if (worksheet == null)
                            {
                                return "Format template salah";
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
                                    return "Ada data pada baris " + row + " yang masih kosong";
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
                                    CreatorUsername = "admin",
                                    LastModifierUsername = "admin",
                                    LastModificationTime = DateTime.Now,
                                    DeleterUsername = null,
                                };
                                _questionAppService.Create(quizQuestions);
                            }
                        }
                    }
                }

                return "Berhasil menambah data";
            }
            return "Something went wrong";
        }
        #endregion

        #region Answer
        [HttpGet("/api/services/app/backoffice/LiveQuiz/getAssignedUser")]
        public BaseResponse getAssignedUser(Guid id, [FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id);
            var count = query.Count();
            List<InternalUsersVM> selectedUser = new List<InternalUsersVM>();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x =>
                    x.InternalUser.Nama.Contains(request.Query) ||
                    x.InternalUser.Jabatan.Contains(request.Query) ||
                    x.InternalUser.DealerName.Contains(request.Query) ||
                    x.InternalUser.DealerKota.Contains(request.Query)
                );
            }
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            foreach (var user in data)
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

            return BaseResponse.Ok(selectedUser, count);
        }

        [HttpGet("/api/services/app/backoffice/LiveQuiz/getJabatan")]
        public List<Jabatans> GetJabatan(string channel)
        {
            return _jabatanAppService.GetAll().Where(x => x.Channel == channel || channel == null).ToList();
        }

        [HttpGet("/api/services/app/backoffice/LiveQuiz/getKota")]
        public List<Kotas> GetKota()
        {
            return _kotaAppService.GetAll().ToList();
        }

        [HttpGet("/api/services/app/backoffice/LiveQuiz/getDealer")]
        public List<Dealers> GetDealer(string Kota)
        {
            return _dealerAppService.GetDealersBackoffice(Kota).ToList();
        }

        [HttpPost("/api/services/app/backoffice/LiveQuiz/insertUser")]
        public List<LiveQuizAssignments> InsertUser([FromForm]Guid id, [FromForm]List<int> selectedUser)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).ToList();

            if (assignedUser.Count > 0)
            {
                List<LiveQuizAssignments> deletedItem = new List<LiveQuizAssignments>();
                var listInserted = selectedUser.Where(p => !assignedUser.Any(l => p == l.IDMPM));

                foreach (var inserted in listInserted)
                {
                    LiveQuizAssignments assigned = new LiveQuizAssignments
                    {
                        Id = Guid.NewGuid(),
                        IDMPM = inserted,
                        LiveQuizId = id
                    };
                    
                    _assignmentAppService.Create(assigned);
                }
            }
            else
            {
                foreach (var user in selectedUser)
                {
                    LiveQuizAssignments assigned = new LiveQuizAssignments
                    {
                        Id = Guid.NewGuid(),
                        IDMPM = user,
                        LiveQuizId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }

            return _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).ToList();
        }

        [HttpPost("/api/services/app/backoffice/LiveQuiz/insertAllUser")]
        public List<LiveQuizAssignments> InsertAllUser(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).ToList();
            List<InternalUsersVM> allUser = _internalUserAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Select(x => new InternalUsersVM
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
            }
            else
            {
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

            return assignedUser;
        }

        [HttpPost("/api/services/app/backoffice/LiveQuiz/removeUser")]
        public String RemoveUser([FromForm]Guid id, [FromForm]List<int> selectedUser)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).ToList();

            if (assignedUser.Count > 0)
            {
                List<LiveQuizAssignments> deletedItem = new List<LiveQuizAssignments>();
                //var listInserted = selectedUser.Where(p => !assignedUser.Any(l => p.Idmpm == l.IDMPM));
                var listDeleted = assignedUser.Where(p => selectedUser.Any(l => p.IDMPM == l));

                foreach (var deleted in listDeleted)
                {
                    _assignmentAppService.Delete(deleted.Id);
                }
            }

            return "Proses berhasil";
        }

        [HttpDelete("/api/services/app/backoffice/LiveQuiz/removeAllUser")]
        public String RemoveAllUser(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.LiveQuizId == id).ToList();
            List<InternalUsersVM> allUser = _internalUserAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Select(x => new InternalUsersVM
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
                var listDeleted = assignedUser.Where(p => allUser.Any(l => p.IDMPM == l.Idmpm));

                foreach (var deleted in listDeleted)
                {
                    _assignmentAppService.Delete(deleted.Id);
                }
            }

            return "Proses berhasil";
        }
        #endregion

        #region Score
        [HttpGet("/api/services/app/backoffice/LiveQuiz/getHistories")]
        public BaseResponse GetHistories(Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _historyAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
            if (!string.IsNullOrEmpty(request.ParentId)){
                query = query.Where(x=> x.LiveQuizId.ToString() == request.ParentId);
            }
            var count = query.Count();

            var data = query.OrderByDescending(x => x.Score).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }
        #endregion
    }
}