using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using MPM.FLP.Authorization.Users;
using System.IO;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Hosting;

namespace MPM.FLP.Services.Backoffice
{
    public class HomeworkQuizController : FLPAppServiceBase, IHomeworkQuizController
    {
        private readonly UserManager _userManager;
        private readonly HomeworkQuizAppService _appService;
        private readonly HomeworkQuizQuestionAppService _questionAppService;
        private readonly HomeworkQuizAssignmentAppService _assignmentAppService;
        private readonly HomeworkQuizHistoryAppService _historyAppService;
        private readonly HomeworkQuizAnswerAppService _historyAnswerAppService;
        private readonly KotaAppService _kotaAppService;
        private readonly DealerAppService _dealerAppService;
        private readonly InternalUserAppService _internalUserAppService;
        private readonly JabatanAppService _jabatanAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeworkQuizController(HomeworkQuizAppService appService, HomeworkQuizQuestionAppService questionAppService, UserManager userManager, HomeworkQuizAssignmentAppService assignmentAppService, HomeworkQuizHistoryAppService historyAppService, HomeworkQuizAnswerAppService historyAnswerAppService, KotaAppService kotaAppService, DealerAppService dealerAppService, InternalUserAppService internalUserAppService, JabatanAppService jabatanAppService, IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _questionAppService = questionAppService;
            _assignmentAppService = assignmentAppService;
            _userManager = userManager;
            _historyAppService = historyAppService;
            _kotaAppService = kotaAppService;
            _dealerAppService = dealerAppService;
            _internalUserAppService = internalUserAppService;
            _historyAnswerAppService = historyAnswerAppService;
            _jabatanAppService = jabatanAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/getByID")]
        public HomeworkQuizzes GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/HomeworkQuizzes/create")]
        public async Task<HomeworkQuizzes> Create([FromForm]HomeworkQuizzes model, [FromForm]IEnumerable<IFormFile> images)
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

                foreach(var image in images)
                {
                    AzureController azureController = new AzureController();
                    model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(image, model.Id.ToString(), "IMG", "hwquiz");
                }

                _appService.Create(model);
            }
            return model;
        }


        [HttpPut("/api/services/app/backoffice/HomeworkQuizzes/update")]
        public async Task<HomeworkQuizzes> Edit(HomeworkQuizzes model, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);


                foreach (var image in images)
                {
                    AzureController azureController = new AzureController();
                    model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(image, model.Id.ToString(), "IMG", "hwquiz");
                }

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/HomeworkQuizzes/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/getAllQuestions")]
        public BaseResponse GetAllQuestions(Guid guid, [FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _questionAppService.GetAll().Where(x => x.HomeworkQuizId == guid);
            var count = query.Count();

            if(!string.IsNullOrEmpty(request.Query)){
                query = query.Where(x=> request.Query.Contains(x.Question) || request.Query.Contains(x.Question) || request.Query.Contains(x.CreatorUsername));
            }
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/getQuestionByID")]
        public HomeworkQuizQuestions GetQuestionByID(Guid guid)
        {
            return _questionAppService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/HomeworkQuizzes/createQuestion")]
        public async Task<HomeworkQuizQuestions> CreateQuestion([FromForm]HomeworkQuizQuestions model, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.DeleterUsername = "";

                if (images.Count() > 0)
                {
                    model.ImageUrl = await new AzureController().InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "hwquizquestion");
                }
                else
                {
                    model.ImageUrl = "";
                }
                _questionAppService.Create(model);
            }
            
            return model;
        }

        [HttpPut("/api/services/app/backoffice/HomeworkQuizzes/updateQuestion")]
        public async Task<HomeworkQuizQuestions> EditQuestion(HomeworkQuizQuestions model, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);

                if (images.Count() > 0)
                {
                    model.ImageUrl = await new AzureController().InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "hwquizquestion");
                }
                else
                {
                    if (model.ImageUrl == null)
                    {
                        model.ImageUrl = "";
                    }
                    else
                    {
                        model.ImageUrl = model.ImageUrl;
                    }

                }

                _questionAppService.Update(model);
            }
            
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/HomeworkQuizzes/destroyQuestion")]
        public String DestroyQuestion(Guid guid)
        {
            _questionAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/downloadTemplate")]
        public ActionResult DownloadTemplate()
        {
            string excelName = "Homework Quiz Question Template.xlsx";

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

            //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);

            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = excelName
            };
        }

        [HttpPost("/api/services/app/backoffice/HomeworkQuizzes/importExcel")]
        public async Task<String> ImportExcel([FromForm]Guid Id, [FromForm]IEnumerable<IFormFile> files)
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
                            HomeworkQuizQuestions quizQuestions = new HomeworkQuizQuestions
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
                                HomeworkQuizId = Id,
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

        private ActionResult File(MemoryStream stream, string v, string excelName)
        {
            throw new NotImplementedException();
        }

        #region Assign User
        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/getAssignedUser")]
        public BaseResponse getAssignedUser(Guid id, [FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            request = Paginate.Validate(request);var query = _assignmentAppService.GetAll().Where(x => x.HomeworkQuizId == id);
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
                    IDGroupJabatan = _internalUserAppService.GetAll().SingleOrDefault(x => x.IDMPM == user.IDMPM).IDGroupJabatan,

                };
                selectedUser.Add(assigned);
            }

            return BaseResponse.Ok(selectedUser, count);
            /*
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.HomeworkQuizId == id).Take(100).ToList();
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
                    IDGroupJabatan = _internalUserAppService.GetAll().SingleOrDefault(x => x.IDMPM == user.IDMPM).IDGroupJabatan,

                };
                selectedUser.Add(assigned);
            }

            return selectedUser;
            */
        }

        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/getJabatan")]
        public List<Jabatans> GetJabatan(string channel)
        {
            return _jabatanAppService.GetAll().Where(x => x.Channel == channel || channel == null).ToList();
        }

        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/getKota")]
        public List<Kotas> getKota()
        {
            return _kotaAppService.GetAll().ToList();
        }

        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/getDealer")]
        public List<Dealers> getDealer(string Kota)
        {
            return _dealerAppService.GetDealersBackoffice(Kota).ToList();
        }

        /*
        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/getAssignedUser")]
        public async Task<List<HomeworkQuizAssignments>> getAssignedUser(string id)
        {
            var task = _internalUserAppService.GetAll();

            var assignedUser = _assignmentAppService.GetAll().Where(x => x.HomeworkQuizId == Guid.Parse(id)).Select(x => x.IDMPM).ToList();
            task = await Task.Run(() => task.Where(x => assignedUser.Contains(x.IDMPM)));
            var assignedUserList = _assignmentAppService.GetAll().Where(x => x.HomeworkQuizId == Guid.Parse(id)).ToList();

            return assignedUserList;
        }
        */

        [HttpPost("/api/services/app/backoffice/HomeworkQuizzes/assignUser")]
        public List<HomeworkQuizAssignments> InsertUser([FromForm]Guid id, [FromForm]List<int> selectedUser)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.HomeworkQuizId == id).ToList();
            
            if (assignedUser.Count > 0)
            {
                var listInserted = selectedUser.Where(Idmpm => !assignedUser.Any(l => Idmpm == l.IDMPM));
                
                foreach (var Idmpm in listInserted)
                {
                    HomeworkQuizAssignments assigned = new HomeworkQuizAssignments
                    {
                        Id = Guid.NewGuid(),
                        IDMPM = Idmpm,
                        HomeworkQuizId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }
            else
            {
                foreach (var Idmpm in selectedUser)
                {
                    HomeworkQuizAssignments assigned = new HomeworkQuizAssignments
                    {
                        Id = Guid.NewGuid(),
                        IDMPM = Idmpm,
                        HomeworkQuizId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }

            return _assignmentAppService.GetAll().Where(x => x.HomeworkQuizId == id).ToList();
        }

        [HttpPost("/api/services/app/backoffice/HomeworkQuizzes/assignAllUser")]
        public List<HomeworkQuizAssignments> InsertAllUser(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.HomeworkQuizId == id).ToList();
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
                var listInserted = allUser.Where(p => !assignedUser.Any(l => p.Idmpm == l.IDMPM));
                
                foreach (var inserted in listInserted)
                {
                    HomeworkQuizAssignments assigned = new HomeworkQuizAssignments
                    {
                        Id = Guid.NewGuid(),
                        IDMPM = inserted.Idmpm,
                        HomeworkQuizId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }
            else
            {
                foreach (var user in allUser)
                {
                    HomeworkQuizAssignments assigned = new HomeworkQuizAssignments
                    {
                        Id = Guid.NewGuid(),
                        IDMPM = user.Idmpm,
                        HomeworkQuizId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }

            return assignedUser;
        }

        [HttpPost("/api/services/app/backoffice/HomeworkQuizzes/removeUser")]
        public string RemoveUser([FromForm]Guid id, [FromForm]List<int> selectedUser)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.HomeworkQuizId == id).ToList();

            if (assignedUser.Count > 0)
            {
                List<HomeworkQuizAssignments> deletedItem = new List<HomeworkQuizAssignments>();
                //var listInserted = selectedUser.Where(p => !assignedUser.Any(l => p.Idmpm == l.IDMPM));
                var listDeleted = assignedUser.Where(p => selectedUser.Any(l => p.IDMPM == l));

                foreach (var deleted in listDeleted)
                {
                    _assignmentAppService.Delete(deleted.Id);
                }
            }

            return "success remove user";
        }

        [HttpDelete("/api/services/app/backoffice/HomeworkQuizzes/removeAllUser")]
        public List<HomeworkQuizAssignments> RemoveAllUser(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedUser = _assignmentAppService.GetAll().Where(x => x.HomeworkQuizId == id).ToList();
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
                var listDeleted = assignedUser.Where(p => allUser.Any(l => p.IDMPM == l.Idmpm));

                foreach (var deleted in listDeleted)
                {
                    _assignmentAppService.Delete(deleted.Id);
                }
            }

            return assignedUser;
        }
        #endregion

        #region Score
        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/historyScore")]
        public List<HomeworkQuizHistories> Grid_Read_HwQuizScore(Pagination request)
        {
            var result = _historyAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.HomeworkQuizId.ToString() == request.ParentId).OrderByDescending(x => x.Score).ToList();
            if (result != null){    
                foreach (var item in result)
                {
                    if (item.CorrectAnswer > item.HomeworkQuiz.HomeworkQuizQuestions.Count())
                    {
                        item.CorrectAnswer = item.HomeworkQuiz.HomeworkQuizQuestions.Count();
                        item.WrongAnswer = 0;
                        item.Score = 100;
                    }
                }
            }
            return result;
        }

        [HttpGet("/api/services/app/backoffice/HomeworkQuizzes/downloadScore")]
        public async Task<ActionResult> DownloadScore(Guid id)
        {
            var model = _appService.GetById(id);
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Homework Quiz " + model.Title + ".xlsx";

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
                    workSheet2.Cells[1, 4].Value = "Kunci Jawaban";

                    var hwQuizScore = _historyAppService.GetAll().Where(x => x.HomeworkQuizId == id && string.IsNullOrEmpty(x.DeleterUsername)).ToList();

                    int row = 2;
                    int detailRow = 2;

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

                        foreach (var detail in score.HomeworkQuizAnswers.OrderBy(x => x.CreationTime))
                        {
                            workSheet2.Cells[detailRow, 1].Value = score.IDMPM;
                            workSheet2.Cells[detailRow, 2].Value = detail.Question;
                            workSheet2.Cells[detailRow, 3].Value = detail.Answer;
                            workSheet2.Cells[detailRow, 4].Value = detail.HomeworkQuizHistory.HomeworkQuiz.HomeworkQuizQuestions.FirstOrDefault(x => x.Question == detail.Question).CorrectOption;
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

                    workSheet2.Column(1).AutoFit();
                    workSheet2.Column(2).AutoFit();
                    package.Save();
                }

                stream.Flush();
                stream.Position = 0;

                //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = excelName
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
            }

            return null;
        }
        #endregion
    }
}