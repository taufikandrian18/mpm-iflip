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

namespace MPM.FLP.Services.Backoffice
{
    public class RoleplayController : FLPAppServiceBase, IRoleplayController
    {
        private readonly RolePlayAppService _appService;
        private readonly RolePlayDetailAppService _detailAppService;
        private readonly RolePlayResultAppService _resultAppService;
        private readonly RolePlayResultDetailAppService _detailResultAppService;
        private readonly RolePlayAssignmentAppService _assignmentAppService;
        private readonly DealerAppService _dealerAppService;
        private readonly KotaAppService _kotaAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public RoleplayController(RolePlayAppService appService, RolePlayDetailAppService detailAppService, RolePlayResultAppService resultAppService, RolePlayResultDetailAppService detailResultAppService, RolePlayAssignmentAppService assignmentAppService, DealerAppService dealerAppService, KotaAppService kotaAppService, IHostingEnvironment hostingEnvironment)
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

        [HttpGet("/api/services/app/backoffice/Roleplay/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/Roleplay/getByID")]
        public RolePlays GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/Roleplay/create")]
        public async Task<RolePlays> Create([FromForm]RolePlays model, [FromForm]IEnumerable<IFormFile> images)
        {
            if(model != null)
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

        [HttpPut("/api/services/app/backoffice/Roleplay/update")]
        public RolePlays Edit(RolePlays model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/Roleplay/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        #endregion

        #region Detail
        [HttpGet("/api/services/app/backoffice/Roleplay/getAllDetails")]
        public BaseResponse GetAllDetails([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _detailAppService.GetAll();

            if(!string.IsNullOrEmpty(request.ParentId)){
                query = query.Where(x=> x.RolePlayId.ToString() == request.ParentId);
            }

            if (!string.IsNullOrEmpty(request.Query))
            {
               query = query.Where(x => 
                    x.Title.Contains(request.Query) || 
                    x.CreatorUsername.Contains(request.Query) || 
                    x.RolePlayId.ToString() == request.Query
                );
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/Roleplay/getDetailByID")]
        public RolePlayDetails GetDetailByID(Guid guid)
        {
            return _detailAppService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/Roleplay/createDetail")]
        public async Task<RolePlayDetails> CreateDetail([FromForm]RolePlayDetails model)
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

        [HttpPut("/api/services/app/backoffice/Roleplay/updateDetail")]
        public RolePlayDetails EditDetail(RolePlayDetails model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _detailAppService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/Roleplay/destroyDetail")]
        public String DestroyDetail(Guid guid)
        {
            _detailAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
        #endregion

        #region Assign Dealer
        [HttpGet("/api/services/app/backoffice/Roleplay/getAssignedDealer")]
        public List<DealerVM> GetAssignedDealer(Guid id)
        {
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.RolePlayId == id).ToList();
            List<DealerVM> selectedDealer = new List<DealerVM>();

            foreach (var dealer in assignedDealer)
            {
                DealerVM assigned = new DealerVM
                {
                    nama = dealer.NamaDealer,
                    kodedealermpm = dealer.KodeDealerMPM,
                    id = dealer.Id
                };
                selectedDealer.Add(assigned);
            }

            return selectedDealer;
        }

        [HttpGet("/api/services/app/backoffice/Roleplay/getKota")]
        public List<Kotas> getKota()
        {
            return _kotaAppService.GetAll().ToList();
        }

        [HttpPost("/api/services/app/backoffice/Roleplay/insertDealer")]
        public List<RolePlayAssignments> InsertDealer([FromForm]Guid id, [FromForm]List<string> selectedDealer)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.RolePlayId == id).ToList();

            if (assignedDealer.Count > 0)
            {
                List<RolePlayAssignments> deletedItem = new List<RolePlayAssignments>();

                var listInserted = selectedDealer.Where(p => !assignedDealer.Any(l => p == l.KodeDealerMPM));

                foreach (var inserted in listInserted)
                {
                    var dealer = _dealerAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == inserted);

                    RolePlayAssignments assigned = new RolePlayAssignments
                    {
                        Id = Guid.NewGuid(),
                        KodeDealerMPM = dealer.KodeDealerMPM,
                        NamaDealer = dealer.Nama,
                        RolePlayId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }
            else
            {
                foreach (var dealer in selectedDealer)
                {
                    var assignDealer = _dealerAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == dealer);

                    RolePlayAssignments assigned = new RolePlayAssignments
                    {
                        Id = Guid.NewGuid(),
                        KodeDealerMPM = assignDealer.KodeDealerMPM,
                        NamaDealer = assignDealer.Nama,
                        RolePlayId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }

            return assignedDealer;
        }

        [HttpPost("/api/services/app/backoffice/Roleplay/insertAllDealer")]
        public List<RolePlayAssignments> InsertAllDealer(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.RolePlayId == id).ToList();
            List<DealerVM> allDealer = _dealerAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Select(x => new DealerVM
            {
                kodedealermpm = x.KodeDealerMPM,
                nama = x.Nama,
            }).ToList();

            if (assignedDealer.Count > 0)
            {
                List<RolePlayAssignments> deletedItem = new List<RolePlayAssignments>();

                var listInserted = allDealer.Where(p => !assignedDealer.Any(l => p.kodedealermpm == l.KodeDealerMPM));
                foreach (var inserted in listInserted)
                {
                    var dealer = _dealerAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == inserted.kodedealermpm);

                    RolePlayAssignments assigned = new RolePlayAssignments
                    {
                        Id = Guid.NewGuid(),
                        KodeDealerMPM = dealer.KodeDealerMPM,
                        NamaDealer = dealer.Nama,
                        RolePlayId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }
            else
            {
                foreach (var dealer in allDealer)
                {
                    var assignDealer = _dealerAppService.GetAll().FirstOrDefault(x => x.KodeDealerMPM == dealer.kodedealermpm);

                    RolePlayAssignments assigned = new RolePlayAssignments
                    {
                        Id = Guid.NewGuid(),
                        KodeDealerMPM = assignDealer.KodeDealerMPM,
                        NamaDealer = assignDealer.Nama,
                        RolePlayId = id
                    };

                    _assignmentAppService.Create(assigned);
                }
            }


            return assignedDealer;
        }

        [HttpPost("/api/services/app/backoffice/Roleplay/removeDealer")]
        public String RemoveDealer([FromForm]Guid id, [FromForm]List<string> selectedDealer)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.RolePlayId == id).ToList();

            if (assignedDealer.Count > 0)
            {
                List<RolePlayAssignments> deletedItem = new List<RolePlayAssignments>();

                var listDeleted = assignedDealer.Where(p => selectedDealer.Any(l => p.KodeDealerMPM == l));

                foreach (var deleted in listDeleted)
                {
                    _assignmentAppService.Delete(deleted.Id);
                }
            }

            return "Proses berhasil";
        }

        [HttpPost("/api/services/app/backoffice/Roleplay/removeAllDealer")]
        public String RemoveAllDealer(Guid id)
        {
            var model = _appService.GetById(id);
            var assignedDealer = _assignmentAppService.GetAll().Where(x => x.RolePlayId == id).ToList();
            List<DealerVM> allDealer = _dealerAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Select(x => new DealerVM
            {
                //id = Guid.Parse(x.Id),
                kodedealermpm = x.KodeDealerMPM,
                nama = x.Nama,
            }).ToList();
            if (assignedDealer.Count > 0)
            {
                List<RolePlayAssignments> deletedItem = new List<RolePlayAssignments>();

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

        #region Result
        [HttpGet("/api/services/app/backoffice/Roleplay/results")]
        public AssignmentDealerVM Result(Guid id)
        {
            var tmp = _assignmentAppService.GetById(id);
            AssignmentDealerVM model = new AssignmentDealerVM
            {
                Id = tmp.RolePlayId,
                Title = _appService.GetById(tmp.RolePlayId).Title,
                KodeDealerMPM = tmp.KodeDealerMPM,
                NamaDealer = tmp.NamaDealer
            };
            return model;
        }

        [HttpGet("/api/services/app/backoffice/Roleplay/resultDetails")]
        public List<RoleplayResultVM> Grid_Result_Read(AssignmentDealerVM roleplay)
        {
            var tmp = _resultAppService.GetAll().Where(x => x.RolePlayId == roleplay.Id && x.KodeDealerMPM == roleplay.KodeDealerMPM && string.IsNullOrEmpty(x.DeleterUsername))
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

        [HttpGet("/api/services/app/backoffice/Roleplay/resultDetail")]
        public List<RoleplayResultVM> Grid_Result_Detail_Read(string idmpm, Guid idRoleplay)
        {
            var id = int.Parse(idmpm);

            var tmp = _resultAppService.GetAll().Where(x => x.IDMPM == id && string.IsNullOrEmpty(x.DeleterUsername) && x.RolePlayId == idRoleplay)
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

        #region Validation
        [HttpPost("/api/services/app/backoffice/Roleplay/calculatePoint")]
        public async Task<Double> CalculatePoint([FromBody]List<ValidationVM> validations, Guid id)
        {
            double total = 0;
            double totalDetail = 0;
            List<RolePlayResultDetails> listPass = new List<RolePlayResultDetails>();
            List<string> listNotPass = new List<string>();
            List<string> listDismiss = new List<string>();
            //List<RolePlayDetails> listDetail = new List<RolePlayDetails>();

            bool containSilver = false;
            bool containGold = false;
            bool containPlatinum = false;

            int totalPlatinum = 0;
            int totalGold = 0;
            int totalSilver = 0;

            foreach (var validation in validations)
            {
                //var detail = _detailAppService.GetById(Guid.Parse(validation.Id));
                var result = _detailResultAppService.GetAll().SingleOrDefault(x => x.RolePlayDetailId == Guid.Parse(validation.Id) && x.RolePlayResultId == validation.ResultId);

                if (totalDetail == 0)
                {
                    var tmpResult = _detailResultAppService.GetAll().Where(x => x.RolePlayResultId == validation.ResultId).ToList();

                    foreach (var tmpresult in tmpResult)
                    {
                        totalDetail++;
                    }
                    //listDetail = _detailAppService.GetAll().Where(x => x.RolePlayId == detail.RolePlayId).ToList();
                    totalPlatinum = _detailResultAppService.GetAll().Where(x => x.IsMandatoryPlatinum == true && x.RolePlayResultId == validation.ResultId).Count();
                    totalGold = _detailResultAppService.GetAll().Where(x => x.IsMandatoryGold == true && x.RolePlayResultId == validation.ResultId).Count();
                    totalSilver = _detailResultAppService.GetAll().Where(x => x.IsMandatorySilver == true && x.RolePlayResultId == validation.ResultId).Count();
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

            return Math.Round(total, 2);
        }

        [HttpGet("/api/services/app/backoffice/Roleplay/getMandatory")]
        public async Task<String> GetMandatory(Guid id)
        {
            int totalPlatinum = 0;
            int totalGold = 0;
            int totalSilver = 0;

            var detail = _detailResultAppService.GetAll().Where(x => x.RolePlayResultId == id).ToList();

            totalPlatinum = detail.Where(x => x.IsMandatoryPlatinum == true).Count();
            totalGold = detail.Where(x => x.IsMandatoryGold == true).Count();
            totalSilver = detail.Where(x => x.IsMandatorySilver == true).Count();

            if (totalPlatinum > 0 && totalGold > 0 && totalSilver > 0)
            {
                return "Bronze";
            }
            else if (totalPlatinum > 0 && totalGold > 0)
            {
                return "Silver";
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

        [HttpGet("/api/services/app/backoffice/Roleplay/getValidate")]
        public List<RoleplayResultDetailVM> Grid_Validate_Read(RolePlayResults rolePlayResults)
        {
            List<RoleplayResultDetailVM> details = new List<RoleplayResultDetailVM>();

            var tmpResult = _detailResultAppService.GetAll().Where(x => x.RolePlayResultId == rolePlayResults.Id).ToList();

            foreach (var tmpresult in tmpResult)
            {
                //var detail = _detailAppService.GetAll().SingleOrDefault(x => x.RolePlayId == rolePlayResults.RolePlayId && string.IsNullOrEmpty(x.DeleterUsername) && tmpresult.RolePlayDetailId == x.Id);
                var detail = _detailAppService.GetAll().SingleOrDefault(x => x.RolePlayId == rolePlayResults.RolePlayId && tmpresult.RolePlayDetailId == x.Id);
                if (detail != null)
                {
                    details.Add(new RoleplayResultDetailVM
                    {
                        id = detail.Id,
                        title = detail.Title,
                        order = detail.Order,
                        rolePlayId = detail.RolePlayId,
                        rolePlayResultId = rolePlayResults.Id,

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

        [HttpPost("/api/services/app/backoffice/Roleplay/downloadTemplateAssignee")]
        public ActionResult DownloadTemplateAssignee()
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Roleplay Assignee Template.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("RoleplayAssigneeTemplate");

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

        [HttpPost("/api/services/app/backoffice/Roleplay/importAssignee")]
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
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["RoleplayAssigneeTemplate"];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var dealerName = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var dealerCode = worksheet.Cells[row, 4].Value?.ToString().Trim();

                            if (dealerName == null && dealerCode == null) break;

                            RolePlayAssignments item = new RolePlayAssignments
                            {
                                //Id = Guid.NewGuid(),
                                KodeDealerMPM = dealerCode,
                                NamaDealer = dealerName,
                                RolePlayId = parentId
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