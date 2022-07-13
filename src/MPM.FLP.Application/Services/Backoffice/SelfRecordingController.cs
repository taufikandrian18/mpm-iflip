using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;

namespace MPM.FLP.Services.Backoffice
{
    public class SelfRecordingController : FLPAppServiceBase, ISelfRecordingController
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

        [HttpGet("/api/services/app/backoffice/SelfRecording/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => 
                    x.Title.Contains(request.Query) || 
                    x.CreatorUsername.Contains(request.Query) ||
                    x.Id.ToString() == request.Query
                );
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/SelfRecording/getByID")]
        public SelfRecordings GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/SelfRecording/create")]
        public async Task<SelfRecordings> Create([FromForm]SelfRecordings model, [FromForm]IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";

                _appService.Create(model);
            }
            return model;
        }

        [HttpPut("/api/services/app/backoffice/SelfRecording/create")]
        public SelfRecordings Edit(SelfRecordings model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/SelfRecording/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        #endregion

        #region Result
        [HttpGet("/api/services/app/backoffice/SelfRecording/result")]
        public AssignmentDealerVM Result(Guid id)
        {
            var tmp = _assignmentAppService.GetById(id);
            AssignmentDealerVM model = new AssignmentDealerVM
            {
                Id = tmp.SelfRecordingId,
                Title = _appService.GetById(tmp.SelfRecordingId).Title,
                KodeDealerMPM = tmp.KodeDealerMPM,
                NamaDealer = tmp.NamaDealer
            };
            return model;
        }

        [HttpGet("/api/services/app/backoffice/SelfRecording/resultDetails")]
        public List<RoleplayResultVM> Grid_Result_Read(AssignmentDealerVM item)
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
            return tmp;
        }

        [HttpGet("/api/services/app/backoffice/SelfRecording/resultDetail")]
        public List<RoleplayResultVM> Grid_Result_Detail_Read(string idmpm, Guid idRoleplay)
        {
            var id = int.Parse(idmpm);
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
            return tmp;
        }
        #endregion

        #region Detail
        [HttpGet("/api/services/app/backoffice/SelfRecording/getDetail")]
        public BaseResponse Detail([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _detailAppService.GetAll();

            if(!string.IsNullOrEmpty(request.ParentId)){
                query = query.Where(x=> x.SelfRecordingId.ToString() == request.ParentId);
            }

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => 
                    x.Title.Contains(request.Query) || 
                    x.CreatorUsername.Contains(request.Query) || 
                    x.SelfRecordingId.ToString() == request.Query
                );
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }


        [HttpPost("/api/services/app/backoffice/SelfRecording/createDetail")]
        public async Task<SelfRecordingDetails> CreateDetail(SelfRecordingDetails model)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";

                _detailAppService.Create(model);
            }
            return model;
        }

        [HttpPut("/api/services/app/backoffice/SelfRecording/updateDetail")]
        public SelfRecordingDetails EditDetail(SelfRecordingDetails model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _detailAppService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/SelfRecording/destroyDetail")]
        public String DestroyDetail(Guid guid)
        {
            _detailAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/SelfRecording/getKota")]
        public List<Kotas> Cascading_Get_Kota()
        {
            return _kotaAppService.GetAll().ToList();
        }

        [HttpPost("/api/services/app/backoffice/SelfRecording/downloadTemplate")]
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
                    validation.Formula.Values.Add("Yes");
                    validation.Formula.Values.Add("No");

                    validation = workSheet.DataValidations.AddListValidation("C2:C1000");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    validation.ErrorTitle = "An invalid value was entered";
                    validation.Error = "Select a value from the list";
                    validation.Formula.Values.Add("Yes");
                    validation.Formula.Values.Add("No");

                    validation = workSheet.DataValidations.AddListValidation("D2:D1000");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    validation.ErrorTitle = "An invalid value was entered";
                    validation.Error = "Select a value from the list";
                    validation.Formula.Values.Add("Yes");
                    validation.Formula.Values.Add("No");

                    package.Save();
                }
            }
            catch (Exception ex)
            {

            }

            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        private ActionResult File(MemoryStream stream, string v, string excelName)
        {
            throw new NotImplementedException();
        }

        [HttpPost("/api/services/app/backoffice/SelfRecording/import")]
        public async Task<String> ImportExcel([FromForm]SelfRecordings model, [FromForm]string submit, [FromForm]IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var file = files.FirstOrDefault();

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Self Recording Detail Template"];
                            var rowCount = worksheet.Dimension.Rows;
                            
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var judul = worksheet.Cells[row, 1].Value?.ToString().Trim();

                                bool silver = worksheet.Cells[row, 2].Value?.ToString().Trim().ToLower() == "yes" ? true : false;
                                bool gold = worksheet.Cells[row, 3].Value?.ToString().Trim().ToLower() == "yes" ? true : false;
                                bool platinum = worksheet.Cells[row, 4].Value?.ToString().Trim().ToLower() == "yes" ? true : false;
                                var order = worksheet.Cells[row, 5].Value != null ? int.Parse(worksheet.Cells[row, 5].Value.ToString().Trim()) : 0;

                                if (judul == null && silver == false && gold == false && platinum == false && order == 0) break;

                                SelfRecordingDetails item = new SelfRecordingDetails
                                {
                                    Id = Guid.NewGuid(),
                                    Title = judul,
                                    Order = order,
                                    IsMandatorySilver = silver,
                                    IsMandatoryGold = gold,
                                    IsMandatoryPlatinum = platinum,
                                    CreationTime = DateTime.Now,
                                    CreatorUsername = "admin",
                                    LastModifierUsername = "admin",
                                    LastModificationTime = DateTime.Now,
                                    SelfRecordingId = model.Id,
                                    DeleterUsername = ""
                                };
                                _detailAppService.Create(item);
                            }
                        }
                    }
                }
            }
            return "Success Import";
        }
        #endregion

        #region Assign Dealer
        [HttpGet("/api/services/app/backoffice/SelfRecording/getAssignedDealer")]
        public List<SelfRecordingAssignments> Get_Assigned_Dealer(string id, string channel)
        {
            return _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == Guid.Parse(id)).ToList();
        }

        [HttpPost("/api/services/app/backoffice/SelfRecording/insertDealer")]
        public List<SelfRecordingAssignments> InsertDealer(Guid id, List<string> selectedDealer)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id).ToList();


            if (assignedDealer.Count > 0)
            {
                List<RolePlayAssignments> deletedItem = new List<RolePlayAssignments>();

                var listInserted = selectedDealer.Where(p => !assignedDealer.Any(l => p == l.KodeDealerMPM));

                foreach (var inserted in listInserted)
                {
                    var dealer = _dealerAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == inserted);

                    SelfRecordingAssignments assigned = new SelfRecordingAssignments
                    {
                        Id = Guid.NewGuid(),
                        KodeDealerMPM = dealer.KodeDealerMPM,
                        NamaDealer = dealer.Nama,
                        SelfRecordingId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }
            else
            {
                foreach (var dealer in selectedDealer)
                {
                    var assignDealer = _dealerAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == dealer);

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

            return assignedDealer;
        }

        [HttpPost("/api/services/app/backoffice/SelfRecording/insertAllDealer")]
        public List<SelfRecordingAssignments> InsertAllDealer(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id).ToList();
            List<DealerVM> allDealer = _dealerAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Select(x => new DealerVM
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
            }
            else
            {
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

            return assignedDealer;
        }

        [HttpPost("/api/services/app/backoffice/SelfRecording/removeDealer")]
        public String RemoveDealer(Guid id, List<DealerVM> selectedDealer)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id).ToList();

            if (assignedDealer.Count > 0)
            {
                List<RolePlayAssignments> deletedItem = new List<RolePlayAssignments>();

                var listDeleted = assignedDealer.Where(p => selectedDealer.Any(l => p.KodeDealerMPM == l.kodedealermpm));

                foreach (var deleted in listDeleted)
                {
                    _assignmentAppService.Delete(deleted.Id);
                }
            }

            return "Proses berhasil";
        }

        [HttpDelete("/api/services/app/backoffice/SelfRecording/removeAllDealer")]
        public String RemoveAllDealer(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.SelfRecordingId == id).ToList();
            List<DealerVM> allDealer = _dealerAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Select(x => new DealerVM
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
            }

            return "Proses berhasil";
        }
        #endregion

        #region Validation
        [HttpPost("/api/services/app/backoffice/SelfRecording/calculatePoint")]
        public Double CalculatePoint([FromBody] List<ValidationVM> validations, Guid id)
        {
            double total = 0;
            double totalDetail = 0;
            List<SelfRecordingResultDetails> listPass = new List<SelfRecordingResultDetails>();
            List<string> listNotPass = new List<string>();
            List<string> listDismiss = new List<string>();
            //List<SelfRecordingDetails> listDetail = new List<SelfRecordingDetails>();

            bool containSilver = false;
            bool containGold = false;
            bool containPlatinum = false;

            int totalPlatinum = 0;
            int totalGold = 0;
            int totalSilver = 0;

            foreach (var validation in validations)
            {
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

            return Math.Round(total, 2);
        }

        [HttpGet("/api/services/app/backoffice/SelfRecording/getMandatory")]
        public async Task<String> GetMandatory(Guid id)
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
                return "Bronze";
            }
            else if (totalPlatinum > 0 && totalGold > 0)
            {
                return  "Silver";
            }
            else if (totalPlatinum > 0)
            {
                return "Gold";
            }
            else if (totalGold > 0)
            {
                return "Silver";
            }
            else if (totalSilver > 0)
            {
                return "Bronze";
            }
            else
            {
                return "Platinum";
            }
        }

        [HttpGet("/api/services/app/backoffice/SelfRecording/getValidate")]
        public List<RoleplayResultDetailVM> Grid_Validate_Read(SelfRecordingResults item)
        {
            List<RoleplayResultDetailVM> details = new List<RoleplayResultDetailVM>();
            var tmpResult = _detailResultAppService.GetAll().Where(x => x.SelfRecordingResultId == item.Id).ToList();

            foreach (var tmpresult in tmpResult)
            {
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

            }
            return details;
        }
        #endregion
        [HttpPost("/api/services/app/backoffice/SelfRecording/downloadTemplateAssignee")]
        public ActionResult DownloadTemplateAssignee()
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Self Recording Assignee Template.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("SelfRecordingAssigneeTemplate");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Dealer Name";
                    workSheet.Cells[1, 2].Value = "Channel";
                    workSheet.Cells[1, 3].Value = "Kota";
                    workSheet.Cells[1, 4].Value = "Dealer Code";

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();

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

        [HttpPost("/api/services/app/backoffice/SelfRecording/importAssignee")]
        public async Task<String> ImportExcelAssignee([FromForm] Guid parentId, [FromForm] IEnumerable<IFormFile> files)
        {
                if (files.Count() > 0)
                {
                    var file = files.FirstOrDefault();

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["SelfRecordingAssigneeTemplate"];
                            var rowCount = worksheet.Dimension.Rows;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var dealerName = worksheet.Cells[row, 1].Value?.ToString().Trim();
                                var dealerCode = worksheet.Cells[row, 4].Value?.ToString().Trim();

                                if (dealerName == null && dealerCode == null) break;

                                SelfRecordingAssignments item = new SelfRecordingAssignments
                                {
                                    //Id = Guid.NewGuid(),
                                    KodeDealerMPM = dealerCode,
                                    NamaDealer = dealerName,
                                    SelfRecordingId = parentId
                                };
                                _assignmentAppService.Create(item);
                            }
                        }
                    }
                }
            return "Success Import";
        }
    }
}