using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.EFPlus;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Repositories;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class PointAppService : FLPAppServiceBase, IPointAppService
    {
        private readonly IPointConfigurationRepository _pointConfigurationRepository;
        private readonly IRepository<Points, Guid> _pointRepository;
        private readonly IRepository<ActivityLogs, Guid> _activityLogRepository;

        private readonly IRepository<Articles, Guid> _articleRepository;
        private readonly IRepository<Guides, Guid> _guideRepository;

        private readonly IRepository<SalesPrograms, Guid> _salesProgramRepository;
        private readonly IRepository<ServicePrograms, Guid> _serviceProgramRepository;

        private readonly IRepository<SalesTalks, Guid> _salesTalkRepository;
        private readonly IRepository<ServiceTalkFlyers, Guid> _serviceTalkFlyerRepository;

        private readonly IRepository<SalesIncentivePrograms, Guid> _salesIncentiveProgramRepository;
        private readonly IRepository<BrandCampaigns, Guid> _brandCampaignRepository;

        private readonly IRepository<CSChampionClubs, Guid> _csChampionClubRepository;
        private readonly IRepository<InfoMainDealers, Guid> _infoMainDealerRepository;

        private readonly IRepository<HomeworkQuizzes, Guid> _homeworkQuizRepository;
        private readonly IRepository<LiveQuizzes, Guid> _liveRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public PointAppService(
            IPointConfigurationRepository pointConfigurationRepository,
            IRepository<Points, Guid> pointRepository,
            IRepository<ActivityLogs, Guid> activityLogRepository,

            IRepository<Articles, Guid> articleRepository,
            IRepository<Guides, Guid> guideRepository,

            IRepository<SalesPrograms, Guid> salesProgramRepository,
            IRepository<ServicePrograms, Guid> serviceProgramRepository,

            IRepository<SalesTalks, Guid> salesTalkRepository,
            IRepository<ServiceTalkFlyers, Guid> serviceTalkFlyerRepository,

            IRepository<SalesIncentivePrograms, Guid> salesIncentiveProgramRepository,
            IRepository<BrandCampaigns, Guid> brandCampaignRepository,

            IRepository<CSChampionClubs, Guid> csChampionClubRepository,
            IRepository<InfoMainDealers, Guid> infoMainDealerRepository,

            IRepository<HomeworkQuizzes, Guid> homeworkQuizRepository,
            IRepository<LiveQuizzes, Guid> liveRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService
        )
        {
            _pointConfigurationRepository = pointConfigurationRepository;
            _pointRepository = pointRepository;
            _activityLogRepository = activityLogRepository;

            _articleRepository = articleRepository;
            _guideRepository = guideRepository;

            _salesProgramRepository = salesProgramRepository;
            _serviceProgramRepository = serviceProgramRepository;

            _salesTalkRepository = salesTalkRepository;
            _serviceTalkFlyerRepository = serviceTalkFlyerRepository;

            _csChampionClubRepository = csChampionClubRepository;
            _infoMainDealerRepository = infoMainDealerRepository;

            _salesIncentiveProgramRepository = salesIncentiveProgramRepository;
            _brandCampaignRepository = brandCampaignRepository;

            _homeworkQuizRepository = homeworkQuizRepository;
            _liveRepository = liveRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        /// <summary>
        /// Check whether point for specified activity already submitted
        /// </summary>
        /// <param name="activityLogId"></param>
        /// <returns></returns>
        private bool CheckActivityPointExists(ActivityLogs activityLog, string userId)
        {
            var listActivityLogsId = _activityLogRepository.GetAll().Where(x => x.ContentId == activityLog.ContentId && x.ActivityType == activityLog.ActivityType && x.Username==userId).Select(x => x.Id);
            var pointIdExist = _pointRepository.GetAll().Where(x => listActivityLogsId.Contains(x.ActivityLogId)).Select(x => x.Id).Any();

            return pointIdExist;
        }

        #region Mobile

        public async Task AddPoint(Guid activityLogId)
        {
            var currentUser = await GetCurrentUserAsync();
            ActivityLogs activityLog = _activityLogRepository.Get(activityLogId);
            if (CheckActivityPointExists(activityLog, currentUser.UserName)) return;

            var pointConfigurationTask = GetContentPointConfiguration(new PointConfigurationActivityContentDto()
            {
                ActivityType = activityLog.ActivityType,
                ContentType = activityLog.ContentType,
                ContentId = activityLog.ContentId
            });

            var pointConfiguration = await pointConfigurationTask;
            Points point = new Points()
            {
                Username = currentUser.UserName,
                ActivityLogId = activityLogId,
                Point = pointConfiguration.Point
            };

            if (point.Point == 0) return;
            await _pointRepository.InsertAsync(point);
        }

        public async Task<int> GetCurrentPoint()
        {
            var currentUser = await GetCurrentUserAsync();
            int point = await _pointRepository.GetAll()
                .Where(x => x.Username == currentUser.UserName && DateTime.Now <= x.ExpiryTime)
                .SumAsync(x => x.Point);

            return point;
        }

        public async Task<PagedResultDto<PointDto>> GetPointHistory([Required]int page = 1, [Required]int count = 10)
        {
            var currentUser = await GetCurrentUserAsync();
            var points = _pointRepository
                .GetAllIncluding(x => x.ActivityLog)
                .Where(x => x.Username == currentUser.UserName && DateTime.Now <= x.ExpiryTime);

            var totalCountTask = points.CountAsync();
            var pagedPointHistoryTask = points.OrderByDescending(x => x.ActivityLog.Time).Skip((page - 1) * count).Take(count).ToListAsync();

            int totalCount = await totalCountTask;
            var pagedPointHistory = await pagedPointHistoryTask;

            var listPointHistory = new PagedResultDto<PointDto>(totalCount, ObjectMapper.Map<List<PointDto>>(pagedPointHistory));
            return await Task.FromResult(listPointHistory);
        }

        public async Task<ContentPointConfigurationDto> GetContentPointConfiguration(PointConfigurationActivityContentDto pointConfigurationActivityContentDto)
        {
            PointConfigurationActivityDto pointConfigurationActivityDto = new PointConfigurationActivityDto()
            {
                ActivityType = pointConfigurationActivityContentDto.ActivityType,
                ContentType = pointConfigurationActivityContentDto.ContentType
            };
            var pointConfiguration = await GetPointConfiguration(pointConfigurationActivityDto);
            if (pointConfiguration == null) pointConfiguration = new PointConfigurationDto();

            ContentPointConfigurationDto pointConfigurationDto = new ContentPointConfigurationDto
            {
                Point = pointConfiguration.Point,
                Threshold = pointConfiguration.DefaultThreshold
            };

            // TODO: Add override threshold minutes from individual content
            int? contentThreshold = null;
            switch (pointConfigurationActivityContentDto.ContentType)
            {
                case "article":
                    contentThreshold = _articleRepository.GetAll().Where(x => x.Id.ToString() == pointConfigurationActivityContentDto.ContentId).FirstOrDefault()?.ReadingTime;
                    break;
                case "guide":
                    contentThreshold = _guideRepository.GetAll().Where(x => x.Id.ToString() == pointConfigurationActivityContentDto.ContentId).FirstOrDefault()?.ReadingTime;
                    break;
                case "salesprogram":
                    contentThreshold = _salesProgramRepository.GetAll().Where(x => x.Id.ToString() == pointConfigurationActivityContentDto.ContentId).FirstOrDefault()?.ReadingTime;
                    break;
                case "serviceprogram":
                    contentThreshold = _serviceProgramRepository.GetAll().Where(x => x.Id.ToString() == pointConfigurationActivityContentDto.ContentId).FirstOrDefault()?.ReadingTime;
                    break;
                case "salestalk":
                    contentThreshold = _salesTalkRepository.GetAll().Where(x => x.Id.ToString() == pointConfigurationActivityContentDto.ContentId).FirstOrDefault()?.ReadingTime;
                    break;
                case "servicestalkflyer":
                    contentThreshold = _serviceTalkFlyerRepository.GetAll().Where(x => x.Id.ToString() == pointConfigurationActivityContentDto.ContentId).FirstOrDefault()?.ReadingTime;
                    break;
                case "brandcampaign":
                    contentThreshold = _brandCampaignRepository.GetAll().Where(x => x.Id.ToString() == pointConfigurationActivityContentDto.ContentId).FirstOrDefault()?.ReadingTime;
                    break;
                case "infomaindealer":
                    contentThreshold = _infoMainDealerRepository.GetAll().Where(x => x.Id.ToString() == pointConfigurationActivityContentDto.ContentId).FirstOrDefault()?.ReadingTime;
                    break;
                case "homewrokquiz":
                   var minimaScore = _homeworkQuizRepository.GetAll().Where(x => x.Id.ToString() == pointConfigurationActivityContentDto.ContentId).FirstOrDefault()?.MinimalScore.GetValueOrDefault();
                   contentThreshold = (Int32)minimaScore;
                    break;
                case "livequiz":
                case "roleplay":
                case "selfrecording":
                case "onlinemagazine":
                    break;
            }
            if (contentThreshold.HasValue) pointConfigurationDto.Threshold = contentThreshold.Value;

            return await Task.FromResult(pointConfigurationDto);
        }
        #endregion

        #region Web
        public async Task<List<PointConfigurationDto>> GetAll()
        {
            var pointConfigurations = await _pointConfigurationRepository.GetAll().ToListAsync();
            var pointConfigurationDto = ObjectMapper.Map<List<PointConfigurationDto>>(pointConfigurations);
            return pointConfigurationDto;
        }

        public IQueryable<PointConfigurations> GetAllBackoffice()
        {
            return _pointConfigurationRepository.GetAll().Where(x => x.DeletionTime == null);
        }

        public PointConfigurations GetById(Guid id)
        {
            return _pointConfigurationRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public async Task<List<PointConfigurationDto>> GetActivePointConfigurations()
        {
            var pointConfigurations = await _pointConfigurationRepository.GetActivePointConfigurations().ToListAsync();
            var pointConfigurationDto = ObjectMapper.Map<List<PointConfigurationDto>>(pointConfigurations);
            return pointConfigurationDto;
        }

        public async Task<AddPointConfigurationDto> AddPointConfiguration(AddPointConfigurationDto addPointConfigurationDto)
        {
            /* make others not default if the added entry is default */
            if (addPointConfigurationDto.IsDefault)
            {
                await _pointConfigurationRepository.BatchUpdateAsync(x => new PointConfigurations()
                {
                    IsDefault = false
                }, x => x.ContentType == addPointConfigurationDto.ContentType &&
                        x.ActivityType == addPointConfigurationDto.ActivityType);
            }

            PointConfigurations pointConfigurations = ObjectMapper.Map<PointConfigurations>(addPointConfigurationDto);
            var currentUser = await GetCurrentUserAsync();
            pointConfigurations.ContentType = pointConfigurations.ContentType.ToLower();
            pointConfigurations.ActivityType = pointConfigurations.ActivityType.ToLower();
            pointConfigurations.CreatorUsername = currentUser.UserName;
            //await _pointConfigurationRepository.InsertAsync(pointConfigurations);
            var pointId = await _pointConfigurationRepository.InsertAndGetIdAsync(pointConfigurations);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, pointConfigurations.CreatorUsername, "Master Menu Point", pointId, pointConfigurations.Point.ToString(), LogAction.Create.ToString(), null, pointConfigurations);

            return addPointConfigurationDto;
        }

        public async Task<UpdatePointConfigurationDto> UpdatePointConfiguration(UpdatePointConfigurationDto updatePointConfigurationDto)
        {
            /* make others not default if the updated entry is default */
            if (updatePointConfigurationDto.IsDefault)
            {
                var pointConfiguration = await _pointConfigurationRepository.GetAsync(updatePointConfigurationDto.Id);
                await _pointConfigurationRepository.BatchUpdateAsync(x => new PointConfigurations()
                {
                    IsDefault = false
                }, x => x.ContentType == pointConfiguration.ContentType &&
                        x.ActivityType == pointConfiguration.ActivityType);
            }

            var currentUser = await GetCurrentUserAsync();
            var oldObject = await _pointConfigurationRepository.GetAsync(updatePointConfigurationDto.Id);

            await _pointConfigurationRepository.BatchUpdateAsync(x => new PointConfigurations()
            {
                Point = updatePointConfigurationDto.Point,
                DefaultThreshold = updatePointConfigurationDto.DefaultThreshold,
                EffDateFrom = updatePointConfigurationDto.EffDateFrom,
                EffDateTo = updatePointConfigurationDto.EffDateTo,
                LastModifierUsername = currentUser.UserName,
                LastModificationTime = DateTime.Now,
                IsDefault = updatePointConfigurationDto.IsDefault
            }, x => x.Id == updatePointConfigurationDto.Id);

            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, currentUser.UserName, "Master Menu Point", updatePointConfigurationDto.Id, updatePointConfigurationDto.Point.ToString(), LogAction.Update.ToString(), oldObject, updatePointConfigurationDto);

            return updatePointConfigurationDto;
        }

        public async Task DeletePointConfiguration(Guid id)
        {
            var currentUser = await GetCurrentUserAsync();
            var oldObject = await _pointConfigurationRepository.GetAsync(id);

            await _pointConfigurationRepository.BatchUpdateAsync(x => new PointConfigurations()
            {
                IsDeleted = true,
                DeleterUsername = currentUser.UserName,
                DeletionTime = DateTime.Now
            }, x => x.Id == id);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, currentUser.UserName, "Master Menu Point", id, oldObject.Id.ToString(), LogAction.Delete.ToString(), oldObject, oldObject);

        }

        public async Task<PointConfigurationDto> GetPointConfigurationById([Required]Guid id)
        {
            var pointConfiguration = await _pointConfigurationRepository.GetAsync(id);
            return ObjectMapper.Map<PointConfigurationDto>(pointConfiguration);
        }

        public async Task<PointConfigurationDto> GetPointConfiguration(PointConfigurationActivityDto getPointConfigurationDto)
        {
            var listPointConfiguration = _pointConfigurationRepository.GetActivePointConfigurations()
                .Where(x => x.ContentType == getPointConfigurationDto.ContentType
                            && x.ActivityType == getPointConfigurationDto.ActivityType);

            var pointConfiguration = await listPointConfiguration.FirstOrDefaultAsync();
            return ObjectMapper.Map<PointConfigurationDto>(pointConfiguration);
        }

        public async Task<List<PointConfigurationDto>> GetPointConfigurationDetailsById([Required]Guid id)
        {
            var pointConfiguration = await _pointConfigurationRepository.GetAsync(id);
            PointConfigurationActivityDto getPointConfigurationDto = ObjectMapper.Map<PointConfigurationActivityDto>(pointConfiguration);
            return await GetPointConfigurationDetails(getPointConfigurationDto);
        }

        public async Task<List<PointConfigurationDto>> GetPointConfigurationDetails(PointConfigurationActivityDto getPointConfigurationDto)
        {
            var pointConfigurations = await _pointConfigurationRepository.GetAll()
                .Where(x => x.ContentType == getPointConfigurationDto.ContentType
                            && x.ActivityType == getPointConfigurationDto.ActivityType
                            && !x.IsDeleted)
                .ToListAsync();

            var listPointConfigurationsDto = ObjectMapper.Map<List<PointConfigurationDto>>(pointConfigurations);
            return listPointConfigurationsDto;
        }
        #endregion
    }
}
