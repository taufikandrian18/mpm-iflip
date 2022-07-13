using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IPointAppService : IApplicationService
    {
        #region Mobile
        /// <summary>
        /// Add Point
        /// </summary>
        /// <param name="activityLogId"></param>
        /// <returns></returns>
        Task AddPoint(Guid activityLogId);

        /// <summary>
        /// Get current user's points
        /// </summary>
        /// <returns></returns>
        Task<int> GetCurrentPoint();

        /// <summary>
        /// Get current user's point history
        /// </summary>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<PagedResultDto<PointDto>> GetPointHistory(int page, int count);

        /// <summary>
        /// Get active Point Configuration for the specified content type and activity type
        /// </summary>
        /// <param name="pointConfigurationActivityContentDto">DTO for Content Type, Content Id, and Activity Type</param>
        /// <returns></returns>
        Task<ContentPointConfigurationDto> GetContentPointConfiguration(PointConfigurationActivityContentDto pointConfigurationActivityContentDto);
        #endregion

        #region Web
        /// <summary>
        /// Get all point configuration 
        /// </summary>
        /// <returns></returns>
        Task<List<PointConfigurationDto>> GetAll();
        IQueryable<PointConfigurations> GetAllBackoffice();
        /// <summary>
        /// Get all active point configuration for each content type and activity type
        /// </summary>
        /// <returns></returns>
        Task<List<PointConfigurationDto>> GetActivePointConfigurations();

        /// <summary>
        /// Add a new Point Configuration
        /// </summary>
        /// <param name="addPointConfigurationDto">DTO for Point Configuration detail</param>
        /// <returns></returns>
        Task<AddPointConfigurationDto> AddPointConfiguration(AddPointConfigurationDto addPointConfigurationDto);

        /// <summary>
        /// Update Point Configuration
        /// </summary>
        /// <param name="updatePointConfigurationDto">DTO for Point Configuration detail</param>
        /// <returns></returns>
        Task<UpdatePointConfigurationDto> UpdatePointConfiguration(UpdatePointConfigurationDto updatePointConfigurationDto);

        /// <summary>
        /// Delete Point Configuration
        /// </summary>
        /// <param name="id">Point Configuration Id</param>
        /// <returns></returns>
        Task DeletePointConfiguration(Guid id);

        /// <summary>
        /// Get Point Configuration for the specified Id
        /// </summary>
        /// <param name="id">Point Configuration Id</param>
        /// <returns></returns>
        Task<PointConfigurationDto> GetPointConfigurationById(Guid id);

        /// <summary>
        /// Get active Point Configuration for the specified content type and activity type
        /// </summary>
        /// <param name="getPointConfigurationDto">DTO for Content Type and Activity Type</param>
        /// <returns></returns>
        Task<PointConfigurationDto> GetPointConfiguration(PointConfigurationActivityDto getPointConfigurationDto);


        /// <summary>
        /// Get all point configuration points and effective dates for content type and activity type of the specified id
        /// </summary>
        /// <param name="id">Point Configuration Id</param>
        /// <returns></returns>
        Task<List<PointConfigurationDto>> GetPointConfigurationDetailsById(Guid id);


        /// <summary>
        /// Get all point configuration points and effective dates for content type and activity type
        /// </summary>
        /// <param name="getPointConfigurationDto">DTO for Content Type and Activity Type</param>
        /// <returns></returns>
        Task<List<PointConfigurationDto>> GetPointConfigurationDetails(PointConfigurationActivityDto getPointConfigurationDto);
        #endregion
    }
}
