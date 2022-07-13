using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Repositories;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services.Backoffice;
using Newtonsoft.Json;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ActivityLogAppService : FLPAppServiceBase, IActivityLogAppService
    {
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IRepository<LogActivities, Guid> _repositoryLog;
        private readonly IRepository<LogActivityDetails, Guid> _repositoryLogDetail;
        private readonly InternalUserAppService _internalUserAppService;


        public ActivityLogAppService(IActivityLogRepository activityLogRepository,  IRepository<LogActivities, Guid> repositoryLog, IRepository<LogActivityDetails, Guid> repositoryLogDetail, InternalUserAppService internalUserAppService)
        {
            _activityLogRepository = activityLogRepository;
            _repositoryLog = repositoryLog;
            _repositoryLogDetail = repositoryLogDetail;
            _internalUserAppService = internalUserAppService;
        }

        #region New API 

        public async Task<BaseResponse> GetSearchActivityLogApp (int year, int month, string selection){
            if(selection == "Activity")
            {
                List<GetActivityLogSummaryDto> result = new List<GetActivityLogSummaryDto>();
                if (year != 0 && month == 0)
                {
                    result = await GetActivityLogsSummaryByYear(year);
                }
                else
                {
                    result = await GetActivityLogsSummaryByMonth(year,month);
                }

                if (result.Count > 0)
                    return BaseResponse.Ok(result,result.Count());
                else
                    return BaseResponse.Ok(result,result.Count());
            }
            else
            {
                List<GetContentActivityLogSummaryDto> result;
                if (year != 0 && month == 0)
                {
                    result = await GetContentActivityLogsSummary(year, null);
                }
                else
                {
                    result = await GetContentActivityLogsSummary(year, month);
                }

                if (result.Count > 0)
                    return BaseResponse.Ok(result,result.Count());
                else
                    return BaseResponse.Ok(result,result.Count());
            }
        }

         public async Task<IActionResult> GetSearchActivityLogAppExportExcel(int year, int month, string selection)
        {
            string excelName = "Activity Log.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Log");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Content Type";
                    
                    

                    if (selection == "Activity")
                    {
                        List<GetActivityLogSummaryDto> result = new List<GetActivityLogSummaryDto>();
                        if (year != 0 && month == 0)
                        {
                            result = await GetActivityLogsSummaryByYear(year);
                        }
                        else
                        {
                            result = await GetActivityLogsSummaryByMonth(year, month);
                        }

                        workSheet.Cells[1, 2].Value = "Activity Type";
                        workSheet.Cells[1, 3].Value = "Count";
                        //workSheet.Cells[1, 4].Value = "Username";
                        workSheet.Cells[1, 4].Value = "Date";

                        int row = 2;
                        foreach (var item in result)
                        {
                            workSheet.Cells[row, 1].Value = item.ContentType;
                            workSheet.Cells[row, 2].Value = item.ActivityType;
                            workSheet.Cells[row, 3].Value = item.Count;
                            workSheet.Cells[row, 4].Value = item.Username;
                            workSheet.Cells[row, 4].Value = item.Time.ToLongDateString();

                            row++;
                        }
                    }
                    else
                    {
                        List<GetContentActivityLogSummaryDto> result = new List<GetContentActivityLogSummaryDto>();
                        if (year != 0 && month == 0)
                        {
                            result = await GetContentActivityLogsSummary(year, null);
                        }
                        else
                        {
                            result = await GetContentActivityLogsSummary(year, month);
                        }

                        workSheet.Cells[1, 2].Value = "Activity Type";
                        //workSheet.Cells[1, 3].Value = "Content Id";
                        workSheet.Cells[1, 3].Value = "Content Title";
                        workSheet.Cells[1, 4].Value = "Count";
                        //workSheet.Cells[1, 5].Value = "Username";
                        workSheet.Cells[1, 5].Value = "Date";

                        int row = 2;
                        foreach (var item in result)
                        {
                            workSheet.Cells[row, 1].Value = item.ContentType;
                            workSheet.Cells[row, 2].Value = item.ActivityType;
                            //workSheet.Cells[row, 3].Value = item.ContentId;
                            workSheet.Cells[row, 3].Value = item.ContentTitle;
                            workSheet.Cells[row, 4].Value = item.Count;
                            //workSheet.Cells[row, 5].Value = item.Username;
                            workSheet.Cells[row, 5].Value = item.Time.ToLongDateString();

                            row++;
                        }
                    }

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();

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

        public async Task<IActionResult> GetDownloadAll(int year, int month, string channel)
        {
            string excelName = "ActivityLogPerson"+ year.ToString() + "-" + month.ToString() +".xlsx";

            var stream = new MemoryStream();
            //List<string> intUsernames = _activityLogAppService.GetAll().Result.Where(x => int.TryParse(x.Username, out int i) == true && x.Time.Year == year).Where(x => x.Time.Month == month).Select(x => x.Username).Distinct().ToList();



            //var internalUsers = await _internalUserAppService.GetInternalUserByChannelIncludeActivity(channel);


            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    #region All Internal User

                    //for(int j = 0; j <= 2; j++) { 
                    //foreach (var internalUser in internalUsers)
                    //{
                        //if(internalUser != null)
                        {
                            var workSheet = package.Workbook.Worksheets.Add("Activities");
                            // workSheet.Row(1).Height = 20;
                            // workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            // workSheet.Row(1).Style.Font.Bold = true;

                            //workSheet.Cells[1, 6].Value = "Nama";
                            //workSheet.Cells[1, 7].Value = "Kode Dealer";
                            //workSheet.Cells[1, 8].Value = "Nama Dealer";
                            //workSheet.Cells[1, 9].Value = "Kota";

                            //workSheet.Cells[2, 6].Value = internalUser.Name;
                            //workSheet.Cells[2, 7].Value = internalUser.KodeDealer;
                            //workSheet.Cells[2, 8].Value = internalUser.NamaDealer;
                            //workSheet.Cells[2, 9].Value = internalUser.Kota;

                            workSheet.Cells[1, 1].Value = "IDMPM";
                            workSheet.Cells[1, 2].Value = "Content Type";
                            workSheet.Cells[1, 3].Value = "Activity Type";
                            workSheet.Cells[1, 4].Value = "Content Title";
                            workSheet.Cells[1, 5].Value = "Reading Time";

                        var activities = await GetAllSpecified( year, month);

                        int i = 2;
                            foreach (var activity in activities)
                            {
                                workSheet.Cells[i, 1].Value = activity.Username;
                                workSheet.Cells[i, 2].Value = activity.ContentType;
                                workSheet.Cells[i, 3].Value = activity.ActivityType;
                                workSheet.Cells[i, 4].Value = activity.ContentTitle;
                                //workSheet.Cells[i, 5].Style.Numberformat.Format = "dd-m-yyyy hh:mm";
                                //workSheet.Cells[i, 4].Formula = "=DATE(2014,10,5)";
                                workSheet.Cells[i, 5].Value = activity.Time;

                                i++;
                            }

                            // workSheet.Column(1).AutoFit();
                            // workSheet.Column(2).AutoFit();
                            // workSheet.Column(3).AutoFit();
                            // workSheet.Column(4).AutoFit();
                            // workSheet.Column(6).AutoFit();
                            // workSheet.Column(7).AutoFit();
                            // workSheet.Column(8).AutoFit();
                            // workSheet.Column(9).AutoFit();
                        }
                    //}
                    #endregion

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
        public BaseResponse GetActivityLog( Pagination request){
            request = Paginate.Validate(request);
            
            var query = _repositoryLog.GetAll().ToList();
            if(request.Key != null && request.Query != null){
                if(request.Key.ToLower() == "pagename"){
                    query = query.Where(x=> request.Query.Contains(x.PageName)).ToList();
                }
                if (request.Key.ToLower() == "action"){
                    query = query.Where(x=> request.Query.Contains(x.Action)).ToList();
                }
            }

            if(request.UserId != null){
                 query = query.Where(x=> x.UserId.ToString() == request.UserId).ToList();

            }

            if(request.StartDate != null && request.EndDate != null){
                query = query.Where(x=> x.CreationTime >= request.StartDate && x.CreationTime <= request.EndDate ).ToList();
            }


            var count = query.Count();

            //var data = query.Take(request.Limit).Skip(request.Page);
            var data = query.Skip(request.Page).Take(request.Limit);
            return BaseResponse.Ok(data, count);
        }

         public BaseResponse GetActivityLogDetail( Pagination request){
            request = Paginate.Validate(request);
            
            var query = _repositoryLog.GetAll().Include(x=> x.LogActivityDetails).FirstOrDefault(x=> x.Id.ToString() == request.ID);

            return BaseResponse.Ok(query,0);
        }

        public IActionResult GetDownloadActivityLog (Pagination request){
             request = Paginate.Validate(request);
            
            var query = _repositoryLog.GetAll().Include(x=> x.LogActivityDetails).ToList();
            if(request.Key != null && request.Query != null){
                if(request.Key.ToLower() == "pagename"){
                    query = query.Where(x=> request.Query.Contains(x.PageName)).ToList();
                }
                if (request.Key.ToLower() == "action"){
                    query = query.Where(x=> request.Query.Contains(x.Action)).ToList();
                }
            }

            if(request.UserId != null){
                 query = query.Where(x=> x.UserId.ToString() == request.UserId).ToList();

            }

            if(request.StartDate != null && request.EndDate != null){
                query = query.Where(x=> x.CreationTime >= request.StartDate && x.CreationTime <= request.EndDate ).ToList();
            }


            var count = query.Count();

            var data = query.Take(request.Limit).Skip(request.Page).ToList();

             string excelName = "ActivityLogs.xlsx";

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Activity Logs");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Page Name";
                    workSheet.Cells[1, 2].Value = "Username";
                    workSheet.Cells[1, 3].Value = "Action";
                    workSheet.Cells[1, 4].Value = "Date";
                    workSheet.Cells[1,5].Value = "Title";

                    int row = 2;
                    foreach(var result in data)
                    {
                        workSheet.Cells[row, 1].Value = result.PageName;
                        workSheet.Cells[row, 2].Value = result.UserName;
                        workSheet.Cells[row, 3].Value = result.Action;
                        workSheet.Cells[row, 4].Value = result.CreationTime.ToLongDateString();
                        workSheet.Cells[row, 5].Value = result.LogActivityDetails.RelatedTitle;
                      
                        row++;
                    }

                    package.Save();
                }
            stream.Position = 0;

            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") 
            { 
                FileDownloadName = excelName
            };  
        }

        public async Task<IActionResult> GetDownloadRegisteredUser(int year, int month, string channel)
        {
            //string excelName = "Activity Log Registered User " + year.ToString() + "-" + month.ToString() + ".xlsx";
            string excelName = "Activity Log Registered User.csv";

            var stream = new MemoryStream();
            //List<string> intUsernames = _activityLogAppService.GetAll().Result.Where(x => int.TryParse(x.Username, out int i) == true && x.Time.Year == year).Where(x => x.Time.Month == month).Select(x => x.Username).Distinct().ToList();
            //List<string> extUsernames = _activityLogAppService.GetAll().Result.Where(x => int.TryParse(x.Username, out int i) == false && x.Time.Year == year).Where(x => x.Time.Month == month).Select(x => x.Username).Distinct().ToList();

            //List<InternalUsers> internalUsers = new List<InternalUsers>();
            //foreach (var item in intUsernames)
            //{
            //    var internalUser = _internalUserAppService.GetInternalUserByChannel(channel).SingleOrDefault(x => x.IDMPM == int.Parse(item));
            //    internalUsers.Add(internalUser);
            //}

            var query = _internalUserAppService.GetAll().Where(x => x.AbpUserId != null).AsQueryable();
            if (year != 0)
            {
                query = query.Where(x => x.CreationTime.Year == year);
            }

            if (month != 0) {
                query = query.Where(x => x.CreationTime.Month == month);
            }

            var internalUsers = query.OrderBy(x => x.IDMPM);

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    #region All Internal User
                    var workSheet = package.Workbook.Worksheets.Add("Registered User");
                    //for(int j = 0; j <= 2; j++) { 
                    int row = 2;
                    foreach (var internalUser in internalUsers)
                    {
                        if (internalUser != null)
                        {
                            
                            workSheet.Row(1).Height = 20;
                            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Row(1).Style.Font.Bold = true;

                            workSheet.Cells[1, 1].Value = "Id MPM";
                            workSheet.Cells[1, 2].Value = "Nama";
                            workSheet.Cells[1, 3].Value = "Id Jabatan";
                            workSheet.Cells[1, 4].Value = "Jabatan";
                            workSheet.Cells[1, 5].Value = "Kode Dealer";
                            workSheet.Cells[1, 6].Value = "Nama Dealer";
                            workSheet.Cells[1, 7].Value = "Kota";
                            workSheet.Cells[1, 8].Value = "No. Hp";

                            workSheet.Cells[row, 1].Value = internalUser.IDMPM;
                            workSheet.Cells[row, 2].Value = internalUser.Nama;
                            workSheet.Cells[row, 3].Value = internalUser.IDJabatan;
                            workSheet.Cells[row, 4].Value = internalUser.Jabatan;
                            workSheet.Cells[row, 5].Value = internalUser.KodeDealerMPM;
                            workSheet.Cells[row, 6].Value = internalUser.DealerName;
                            workSheet.Cells[row, 7].Value = internalUser.DealerKota;
                            workSheet.Cells[row, 8].Value = internalUser.Handphone;

                            workSheet.Column(1).AutoFit();
                            workSheet.Column(2).AutoFit();
                            workSheet.Column(3).AutoFit();
                            workSheet.Column(4).AutoFit();
                            workSheet.Column(6).AutoFit();
                            workSheet.Column(7).AutoFit();
                            workSheet.Column(8).AutoFit();
                            workSheet.Column(9).AutoFit();
                        }

                        row++;
                    }
                    #endregion

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

        #endregion

        #region Private Methods
        private IQueryable<ActivityLogs> GetActivityLogs(int? year = null, int? month = null, string username = null, string contentType = null)
        {
            var activityLogs = _activityLogRepository.GetAll();
            if (year != null) activityLogs = activityLogs.Where(x => x.Time.Year == year.GetValueOrDefault());
            if (month != null) activityLogs = activityLogs.Where(x => x.Time.Month == month.GetValueOrDefault());
            if (!string.IsNullOrEmpty(username)) activityLogs = activityLogs.Where(x => x.Username == username);
            if (!string.IsNullOrEmpty(contentType)) activityLogs = activityLogs.Where(x => x.ContentType == contentType);

            return activityLogs;
        }

        private List<GetActivityLogSummaryDto> SummarizeActivityLogs(IEnumerable<ActivityLogs> activityLogs)
        {
            List<GetActivityLogSummaryDto> activityLogsDto = new List<GetActivityLogSummaryDto>();
            activityLogsDto = activityLogs
                .GroupBy(x => new { x.ContentType, x.ActivityType })
                .Select(x => new GetActivityLogSummaryDto()
                {
                    ContentType = x.Select(y => y.ContentType).FirstOrDefault(),
                    ActivityType = x.Select(y => y.ActivityType).FirstOrDefault(),
                    Count = x.Count()
                }).ToList();
            return activityLogsDto;
        }

        private List<GetContentActivityLogSummaryDto> SummarizeContentActivityLogs(IEnumerable<ActivityLogs> activityLogs)
        {
            List<GetContentActivityLogSummaryDto> activityLogsDto = new List<GetContentActivityLogSummaryDto>();
            activityLogsDto = activityLogs
                .GroupBy(x => new { x.ContentId, x.ContentType, x.ActivityType })
                .Select(x => new GetContentActivityLogSummaryDto()
                {
                    ContentId = x.Select(y => y.ContentId).FirstOrDefault(),
                    ActivityType = x.Select(y => y.ActivityType).FirstOrDefault(),
                    ContentTitle = x.Select(y => y.ContentTitle).FirstOrDefault(),
                    ContentType = x.Select(y => y.ContentType).FirstOrDefault(),
                    Count = x.Count(),
                    Time = x.Select(y => y.Time).FirstOrDefault()
                }).ToList();
            return activityLogsDto;
        }
        #endregion

        #region Mobile
        public async Task<Guid> AddActivityLog(AddActivityLogDto addActivityLogDto)
        {
            var currentUser = await GetCurrentUserAsync();
            var activityLog = ObjectMapper.Map<ActivityLogs>(addActivityLogDto);
            activityLog.ContentType = activityLog.ContentType.ToLower();
            activityLog.ActivityType = activityLog.ActivityType.ToLower();
            activityLog.Username = currentUser.UserName;
            var insertedActivityLog = await _activityLogRepository.InsertAndGetGuidAsync(activityLog);
            return insertedActivityLog;
        }

        public async Task<List<GetActivityLogSummaryDto>> GetUserActivityLogSummaryByYear(int year)
        {
            var currentUser = await GetCurrentUserAsync();
            var activityLogs = await GetActivityLogs(year, username: currentUser.UserName).ToListAsync();
            List<GetActivityLogSummaryDto> activityLogsDto = SummarizeActivityLogs(activityLogs);
            return activityLogsDto;
        }

        public async Task<List<GetActivityLogSummaryDto>> GetUserActivityLogSummaryByMonth(int year, int month)
        {
            var currentUser = await GetCurrentUserAsync();
            var activityLogs = await GetActivityLogs(year, month, currentUser.UserName).ToListAsync();
            List<GetActivityLogSummaryDto> activityLogsDto = SummarizeActivityLogs(activityLogs);
            return activityLogsDto;
        }

        public async Task<List<GetContentActivityLogSummaryDto>> GetUserContentActivityLogSummary(int? year, int? month, string contentType = null)
        {
            var currentUser = await GetCurrentUserAsync();
            var activityLogs = await GetActivityLogs(year, month, username: currentUser.UserName, contentType: contentType).ToListAsync();
            List<GetContentActivityLogSummaryDto> activityLogsDto = SummarizeContentActivityLogs(activityLogs);
            return activityLogsDto;
        }

        public async Task<List<GetContentActivityLogSummaryDto>> GetUserContentActivityLogSummaryByYear(int year, string contentType = null)
        {
            return await GetUserContentActivityLogSummary(year, null, contentType);
        }

        public async Task<List<GetContentActivityLogSummaryDto>> GetUserContentActivityLogSummaryByMonth(int year, int month, string contentType = null)
        {
            return await GetUserContentActivityLogSummary(year, month, contentType);
        }
        #endregion

        #region Web
        public async Task<List<ActivityLogs>> GetAll()
        {
            var activityLogs = await GetActivityLogs().ToListAsync();
            return activityLogs;
        }

        public async Task<List<ActivityLogs>> GetAllSpecified(int year, int month)
        {
            var activityLogs = await _activityLogRepository.GetAll().Where(x=> x.Time.Year == year).Where(x => x.Time.Month == month).ToListAsync();
            return activityLogs;
        }

        public async Task<List<GetActivityLogSummaryDto>> GetActivityLogsSummaryByYear(int year)
        {
            var activityLogs = await GetActivityLogsDTO(year).ToListAsync();
            //List<GetActivityLogSummaryDto> activityLogsDto = SummarizeActivityLogs(activityLogs);
            return activityLogs;
        }

        private IQueryable<GetActivityLogSummaryDto> GetActivityLogsDTO(int? year = null, int? month = null, string username = null, string contentType = null)
        {
            var activityLogs = _activityLogRepository.GetAll();
            if (year != null) activityLogs = activityLogs.Where(x => x.Time.Year == year.GetValueOrDefault());
            if (month != null) activityLogs = activityLogs.Where(x => x.Time.Month == month.GetValueOrDefault());
            if (!string.IsNullOrEmpty(username)) activityLogs = activityLogs.Where(x => x.Username == username);
            if (!string.IsNullOrEmpty(contentType)) activityLogs = activityLogs.Where(x => x.ContentType == contentType);

           var response = activityLogs.GroupBy(x => new { x.ContentType, x.ActivityType }).Select(
                x => new GetActivityLogSummaryDto
                {
                    ContentType = x.Select(y => y.ActivityType).FirstOrDefault(),
                    ActivityType = x.Select(y => y.ContentType).FirstOrDefault(),
                    Count = x.Count(),
                    Username = x.Select(y => y.Username).FirstOrDefault(),
                    Time = x.Select(y => y.Time).FirstOrDefault()
                }
            );
            return response;
        }

        public async Task<List<GetActivityLogSummaryDto>> GetActivityLogsSummaryByMonth(int year, int month)
        {
            var activityLogs = await GetActivityLogsDTO(year, month).ToListAsync();
            //List<GetActivityLogSummaryDto> activityLogsDto = SummarizeActivityLogs(activityLogs);
            return activityLogs;
        }

        public async Task<List<GetContentActivityLogSummaryDto>> GetContentActivityLogsSummary(int? year, int? month, string contentType = null)
        {
            var activityLogs = await GetActivityLogs(year, month, contentType: contentType).ToListAsync();
            List<GetContentActivityLogSummaryDto> activityLogsDto = SummarizeContentActivityLogs(activityLogs);
            return activityLogsDto;
        }

        public async Task<List<GetSummaryActivityLog>> GetSummaryActivityLog(DateTime StartDate, DateTime EndDate)
        {
            if (EndDate < StartDate)
                return null;

            var result = await (from log in _repositoryLog.GetAll()
                                where log.CreationTime >= StartDate && log.CreationTime <= EndDate
                                group log by log.Action into grpLog
                                select new GetSummaryActivityLog
                                {
                                    Action = grpLog.Key,
                                    Count = grpLog.Count()
                                }).ToListAsync();

            foreach(var _result in result)
            {
                var logDetail = await (from log in _repositoryLog.GetAll()
                                       where log.CreationTime >= StartDate
                                            && log.CreationTime <= EndDate
                                            && log.Action == _result.Action
                                       group log by log.PageName into grpLog
                                       select new GetSummaryActivityLogDetail
                                       {
                                            PageName = grpLog.Key,
                                            Count = grpLog.Count()
                                       }).ToListAsync();
    
                if (logDetail.Count() > 0)
                    _result.Details.AddRange(logDetail);
            }

            return result;
        }
        #endregion
    }
}
