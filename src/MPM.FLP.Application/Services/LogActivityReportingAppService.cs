using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class LogActivityReportingAppService : FLPAppServiceBase, ILogActivityReportingAppService
    {
        private readonly IRepository<LogActivities, Guid> _repositoryLog;
        private readonly IRepository<LogActivityDetails, Guid> _repositoryDetail;
        private readonly IRepository<InternalUsers> _internalUserRepository;

        public LogActivityReportingAppService(
            IRepository<LogActivities, Guid> repositoryLog,
            IRepository<LogActivityDetails, Guid> repositoryDetail,
            IRepository<InternalUsers> internalUserRepository)
        {
            _repositoryLog = repositoryLog;
            _repositoryDetail = repositoryDetail;
            _internalUserRepository = internalUserRepository;
        }

        public List<LogActivityReportingDetailDto> ExportExcelDetail(LogActivityReportingFilterDto request)
        {
            var result = (from log in _repositoryLog.GetAll().Where(x => x.DeletionTime == null)
                         join detail in _repositoryDetail.GetAll().Where(x => x.DeletionTime == null)
                         on log.Id equals detail.LogActivityGUID
                         join tmpUser in _internalUserRepository.GetAll()
                         on log.UserId equals tmpUser.AbpUserId into _user
                         from user in _user.DefaultIfEmpty()
                         where (request.StartDate == null || log.CreationTime >= request.StartDate)
                         && (request.EndDate == null || log.CreationTime <= request.EndDate)
                         && (request.UserId == null || log.UserId == request.UserId)
                         && (string.IsNullOrEmpty(request.PageName) || log.PageName == request.PageName)
                         && (string.IsNullOrEmpty(request.LogAction) || log.Action == request.LogAction)
                         select new LogActivityReportingDetailDto
                         {
                             IDMPM = user == null ? 0 : user.IDMPM,
                             Name = log.CreatorUsername,
                             Action = log.Action,
                             Date = log.CreationTime,
                             OldData = detail.JsonDataBefore,
                             ResultData = detail.JsonDataAfter
                         }).ToList();
                         
            return result;
        }

        public List<LogActivityReportingSummaryDto> ExportExcelSummary(LogActivityReportingFilterDto request)
        {
            var result = (from log in _repositoryLog.GetAll().Where(x => x.DeletionTime == null)
                          where (request.StartDate == null || log.CreationTime >= request.StartDate)
                          && (request.EndDate == null || log.CreationTime <= request.EndDate)
                          && (request.UserId == null || log.UserId == request.UserId)
                          && (string.IsNullOrEmpty(request.PageName) || log.PageName == request.PageName)
                          && (string.IsNullOrEmpty(request.LogAction) || log.Action == request.LogAction)
                          group log by new { log.PageName, log.Action } into grpLog
                          select new LogActivityReportingSummaryDto
                          {
                              PageName = grpLog.Key.PageName,
                              Action = grpLog.Key.Action,
                              Total = grpLog.Count()
                          }).ToList();

            return result;
        }
    }
}
