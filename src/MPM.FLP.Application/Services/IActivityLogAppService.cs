using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services
{
    public interface IActivityLogAppService : IApplicationService
    {
        #region Mobile
        /// <summary>
        /// Add Activity Log
        /// </summary>
        /// <param name="addActivityLogDto">ContentId, Content Type, Content Title, and Activity Type</param>
        /// <returns></returns>
        Task<Guid> AddActivityLog(AddActivityLogDto addActivityLogDto);
        /// <summary>
        /// Get User Activity Log summary for specified year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        Task<List<GetActivityLogSummaryDto>> GetUserActivityLogSummaryByYear(int year);
        /// <summary>
        /// Get User Activity Log summary for specified year and month
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Task<List<GetActivityLogSummaryDto>> GetUserActivityLogSummaryByMonth(int year, int month);
        /// <summary>
        /// Get User Activity Log summary per content for specified year and month
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Task<List<GetContentActivityLogSummaryDto>> GetUserContentActivityLogSummary(int? year, int? month, string contentType = null);
        /// <summary>
        /// Get User Activity Log summary per content for specified year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        Task<List<GetContentActivityLogSummaryDto>> GetUserContentActivityLogSummaryByYear(int year, string contentType = null);
        /// <summary>
        /// Get User Activity Log summary per content for specified year and month
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Task<List<GetContentActivityLogSummaryDto>> GetUserContentActivityLogSummaryByMonth(int year, int month, string contentType = null);
        #endregion
        #region Web
        /// <summary>
        /// Get All Activity Log 
        /// </summary>
        /// <returns></returns>
        Task<List<ActivityLogs>> GetAll();
        Task<List<ActivityLogs>> GetAllSpecified(int year, int month);
        /// <summary>
        /// Get Activity Log summary for specified year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        Task<List<GetActivityLogSummaryDto>> GetActivityLogsSummaryByYear(int year);
        /// <summary>
        /// Get Activity Log summary for specified year and month
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Task<List<GetActivityLogSummaryDto>> GetActivityLogsSummaryByMonth(int year, int month);
        /// <summary>
        /// Get Activity Log summary per content
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        Task<List<GetContentActivityLogSummaryDto>> GetContentActivityLogsSummary(int? year, int? month, string contentType = null);

        BaseResponse GetActivityLog( Pagination request);
        BaseResponse GetActivityLogDetail( Pagination request);
        IActionResult GetDownloadActivityLog (Pagination request);
        Task<IActionResult> GetDownloadRegisteredUser(int year, int month, string channel);

        Task<IActionResult> GetDownloadAll(int year, int month, string channel);
        Task<IActionResult> GetSearchActivityLogAppExportExcel(int year, int month, string selection);
        Task<BaseResponse> GetSearchActivityLogApp (int year, int month, string selection);
        #endregion
    }
}
