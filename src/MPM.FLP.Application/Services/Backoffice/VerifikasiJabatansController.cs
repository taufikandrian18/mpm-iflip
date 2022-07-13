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
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public class VerifikasiJabatansController : FLPAppServiceBase, IVerifikasiJabatansController
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

        [HttpGet("/api/services/app/backoffice/VerifikasiJabatans/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => 
                    x.Title.Contains(request.Query) || 
                    x.CreatorUsername.Contains(request.Query) || 
                    x.IDGroupJabatan == request.Query || 
                    x.Id.ToString() == request.Query ||
                    x.PassingScore.ToString() == request.Query
                );
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/VerifikasiJabatans/getByID")]
        public VerifikasiJabatans GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpGet("/api/services/app/backoffice/VerifikasiJabatans/getGroupJabatan")]
        public List<Jabatans> Get_IdGroupJabatan()
        {  
            return _jabatanAppService.GetAll().Where(x=> x.Channel == "H1" && !string.IsNullOrEmpty(x.IDGroupJabatan)).Distinct().ToList().ToList();
        }

        [HttpPost("/api/services/app/backoffice/VerifikasiJabatans/create")]
        public async Task<VerifikasiJabatans> Create([FromForm]VerifikasiJabatans model)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.DeleterUsername = "";

                _appService.Create(model);
            }
            
            return model;
        }

        [HttpPut("/api/services/app/backoffice/VerifikasiJabatans/update")]
        public VerifikasiJabatans Edit(VerifikasiJabatans model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/VerifikasiJabatans/delete")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        #region Histories
        [HttpGet("/api/services/app/backoffice/VerifikasiJabatans/getAllHistories")]
        public List<HomeworkQuizDto> GetAllHistories([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _historyAppService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => 
                    x.Name.Contains(request.Query) || 
                    x.CreatorUsername.Contains(request.Query) || 
                    x.Name.Contains(request.Query)
                );
            }

            var count = query.Count();

           

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            var responseData = new List<HomeworkQuizDto>();
            foreach (var item in data)
            {
                var datum =  new HomeworkQuizDto();
                datum.History = item;
                if (item.IDMPM != null){
                   datum.UserData =  _internalUserAppService.GetById(item.IDMPM.Value).Result;
                }
                responseData.Add(datum);
            }

            return responseData;
        }

        [HttpGet("/api/services/app/backoffice/VerifikasiJabatans/getHistoryById")]
        public HomeworkQuizHistories GetHistoryByID(Guid guid)
        {
            return _historyAppService.GetById(guid);
        }
        #endregion

        #region Homework Question
        [HttpGet("/api/services/app/backoffice/VerifikasiJabatans/getAllQuestion")]
        public BaseResponse GetAllQuestion([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _questionAppService.GetAll().Where(x=> x.DeletionTime == null);

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => request.Query.Contains(x.Question) || request.Query.Contains(x.CreatorUsername));
            }

            if(!string.IsNullOrEmpty(request.ParentId)){
                query = query.Where(x=> x.VerfikasiJabatanId.ToString() == request.ParentId);
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/VerifikasiJabatans/getQuestionById")]
        public VerifikasiJabatanQuestions GetQuestionByID(Guid guid)
        {
            return _questionAppService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/VerifikasiJabatans/createQuestion")]
        public async Task<VerifikasiJabatanQuestions> CreateQuestion(QuestionsVM item, IEnumerable<IFormFile> images)
        {
            VerifikasiJabatanQuestions model = new VerifikasiJabatanQuestions();
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
            
            return model;
        }

        [HttpPut("/api/services/app/backoffice/VerifikasiJabatans/editQuestion")]
        public async Task<VerifikasiJabatanQuestions> EditQuestion(QuestionsVM item, IEnumerable<IFormFile> images)
        {
            VerifikasiJabatanQuestions model = _questionAppService.GetById(item.Id);
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
            
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/VerifikasiJabatans/deleteQuestion")]
        public String DestroyQuestion(Guid guid)
        {
            _questionAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
        #endregion

        [HttpPost("/api/services/app/backoffice/VerifikasiJabatans/importExcelQuestion")]
        public async Task<String> ImportExcel([FromForm]Guid guid, [FromForm]IEnumerable<IFormFile> files)
        {
            if (guid != null)
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
                                    return "Data pertanyaan pada baris " + row + " masih kosong";
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
                                    VerfikasiJabatanId = guid,
                                    CreationTime = DateTime.Now,
                                    CreatorUsername = "admin",
                                    LastModifierUsername = "admin",
                                    LastModificationTime = DateTime.Now,
                                    DeleterUsername = ""
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
    }
}