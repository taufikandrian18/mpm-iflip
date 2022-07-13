using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using CorePush.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.FLP.Repositories;
using MPM.FLP.Services.Dto;
using System.Linq.Dynamic.Core;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class SDMSLogServices : FLPAppServiceBase, ISDMSLogsController
    {
        private readonly IRepository<SDMSLogs, Guid> _appService;
        private readonly IRepository<SDMSMessageDetail, Guid> _sdmsDetail;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;

        public SDMSLogServices(IRepository<SDMSLogs, Guid> sdmsRepo, IRepository<SDMSMessageDetail, Guid> sdmsDetailRepo, IRepository<InternalUsers> internalUserRepository, IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository)
        {
            _appService = sdmsRepo;
            _internalUserRepository = internalUserRepository;
        }

        [HttpGet("/api/services/app/backoffice/SDMSLogs/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            try {
            request = Paginate.Validate(request);

            var query =_appService.GetAll();


            if(!string.IsNullOrEmpty(request.Query)){
                if(request.Key == null){
                    query = query.Where(x=> x.EmployeeName.Contains(request.Query) || x.EnumContentType.Contains(request.Query) 
                    || x.EnumContentTitle.Contains(request.Query) || x.EnumContentActivity.Contains(request.Query));
                }
                else if(request.Key.ToLower() == "employeeid") {
                    query = query.Where(x=> x.EmployeeId.Trim().ToLower() == request.Query.Trim().ToLower());
                } else if(request.Key.ToLower() == "enumcontentactivity") {
                    query = query.Where(x=> x.EnumContentActivity.Trim().ToLower() == request.Query.Trim().ToLower());
                } else if(request.Key.ToLower() == "enumcontenttype") {
                    query = query.Where(x=> x.EnumContentType.Trim().ToLower() == request.Query.Trim().ToLower());
                } 
            }

            if (request.StartDate != null && request.EndDate != null){
                query = query.Where(x=>x.CreationTime >= request.StartDate   &&  x.CreationTime  <= request.EndDate);
            }
           
            var count = query.Count();

            var data = query.OrderByDescending(x=> x.CreationTime).Skip(request.Page).Take(request.Limit).Select(x=> new SDMSLogsDTO{
                Id = x.Id.ToString(),
                EmployeeName = x.EmployeeName,
                EmployeeId = x.EmployeeId,
                EnumContentType = x.EnumContentType,
                EnumContentActivity = x.EnumContentActivity,
                EnumContentId = x.EnumContentId,
                EnumContentTitle = x.EnumContentTitle,
                CreationTime  =x.CreationTime,
                CreatorUsername = x.CreatorUsername,
                MDMCode = x.MPMCode
            }).ToList();

          

            return BaseResponse.Ok(data, count);
            } catch(Exception x){
                return BaseResponse.Error(x.Message, x);
            }
        }

        [HttpGet("/api/services/app/backoffice/SDMSLogs/downloadAll")]
        public IActionResult DownloadAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query =_appService.GetAll();


            if(!string.IsNullOrEmpty(request.Query)){
                if(request.Key == null){
                    query = query.Where(x=> x.EmployeeName.Contains(request.Query) || x.EnumContentType.Contains(request.Query) 
                    || x.EnumContentTitle.Contains(request.Query) || x.EnumContentActivity.Contains(request.Query));
                }
                else if(request.Key.ToLower() == "employeeid") {
                    query = query.Where(x=> x.EmployeeId.Trim().ToLower() == request.Query.Trim().ToLower());
                } else if(request.Key.ToLower() == "enumcontentactivity") {
                    query = query.Where(x=> x.EnumContentActivity.Trim().ToLower() == request.Query.Trim().ToLower());
                } else if(request.Key.ToLower() == "enumcontenttype") {
                    query = query.Where(x=> x.EnumContentType.Trim().ToLower() == request.Query.Trim().ToLower());
                } 
            }

            if (request.StartDate != null && request.EndDate != null){
                query = query.Where(x=>x.CreationTime >= request.StartDate   &&  x.CreationTime  <= request.EndDate);
            }
           
            var count = query.Count();

            var data = query.OrderByDescending(x=> x.CreationTime).Skip(request.Page).Take(request.Limit).Select(x=> new SDMSLogsDTO{
                Id = x.Id.ToString(),
                EmployeeName = x.EmployeeName,
                EmployeeId = x.EmployeeId,
                EnumContentType = x.EnumContentType,
                EnumContentActivity = x.EnumContentActivity,
                EnumContentId = x.EnumContentId,
                EnumContentTitle = x.EnumContentTitle,
                CreationTime  =x.CreationTime,
                CreatorUsername = x.CreatorUsername,
                MDMCode = x.MPMCode,
            }).ToList();



            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("SDMS Logs");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Employee Name";
                    workSheet.Cells[1, 2].Value = "Employee Id";
                    workSheet.Cells[1, 3].Value = "Content Type";
                    workSheet.Cells[1, 4].Value = "Content Activity";
                    workSheet.Cells[1, 5].Value = "Content Id";
                    workSheet.Cells[1, 6].Value = "Content Title";
                    workSheet.Cells[1, 7].Value = "Creation Time";
                    workSheet.Cells[1, 8].Value = "Creator Username";
                    workSheet.Cells[1, 9].Value = "MPM Code";

                    int row = 2;
                    foreach(var result in data)
                    {
                        workSheet.Cells[row, 1].Value = result.EmployeeName;
                        workSheet.Cells[row, 2].Value = result.EmployeeId;
                        workSheet.Cells[row, 3].Value = result.EnumContentType;
                        workSheet.Cells[row, 4].Value = result.EnumContentActivity;
                        workSheet.Cells[row, 5].Value = result.EnumContentId;
                        workSheet.Cells[row, 6].Value = result.EnumContentTitle;
                        workSheet.Cells[row, 7].Value = result.CreationTime.ToLongDateString();
                        workSheet.Cells[row, 8].Value = result.CreatorUsername;
                        workSheet.Cells[row, 9].Value = result.MDMCode;
                      
                        row++;
                    }

                    package.Save();
                }
            stream.Position = 0;

            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") 
            { 
                FileDownloadName = "SDMSLog-"+DateTime.Now+".xlsx"
            };
        }


        [HttpGet("/api/services/app/backoffice/SDMSLogs/getByID")]
        public SDMSLogs GetByIDBackoffice(Guid guid)
        {
            return _appService.GetAll().Where(x=> x.Id == guid).FirstOrDefault();
        }

        [HttpPost("/api/services/app/backoffice/SDMSLogs/create")]
        public SDMSLogs CreateBackoffice(SDMSLogs model)
        {
            try {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername="SYSTEM";
                if (model != null)
                {
                    _appService.Insert(model);
                }
            } catch (Exception x){
                Console.Write(x);
            }

            return model;
        }

        [HttpPut("/api/services/app/backoffice/SDMSLogs/update")]
        public SDMSLogs UpdateBackoffice(SDMSLogs model)
        {
            if (model != null)
            {
                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/SDMSLogs/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            //_appService.Delete(guid, "admin");
            return "Successfully deleted";
        }
    }
}
