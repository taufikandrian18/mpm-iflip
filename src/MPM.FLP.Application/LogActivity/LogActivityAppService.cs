using Abp;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.LogActivity
{
    public class LogActivityAppService : FLPAppServiceBase, ILogActivityAppService
    {
        private readonly IRepository<LogActivities, Guid> _repositoryLog;
        private readonly IRepository<LogActivityDetails, Guid> _repositoryLogDetail;

        public LogActivityAppService(
            IRepository<LogActivities, Guid> repositoryLog,
            IRepository<LogActivityDetails, Guid> repositoryLogDetail)
        {
            _repositoryLog = repositoryLog;
            _repositoryLogDetail = repositoryLogDetail;
        }

        public void CreateLogActivity(long UserId, string UserName, string PageName, Guid RelatedId, string RelatedTitle, string Action, Object OldObject, Object NewObject)
        {
            try
            {
                var logActivity = new LogActivities
                {
                    UserId = UserId,
                    UserName = UserName,
                    PageName = PageName,
                    Action = Action,
                    CreatorUsername = UserName,
                    CreationTime = DateTime.Now
                };
                var logActivityId = _repositoryLog.InsertAndGetId(logActivity);

                _repositoryLogDetail.Insert(new LogActivityDetails
                {
                    LogActivityGUID = logActivityId,
                    RelatedId = RelatedId,
                    RelatedTitle = RelatedTitle,
                    JsonDataBefore = JsonConvert.SerializeObject(OldObject),
                    JsonDataAfter = JsonConvert.SerializeObject(NewObject),
                    CreatorUsername = UserName,
                    CreationTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
            }
        }

        public List<LogActivities> GetByRelatedId (Guid RelatedId)
        {
            return _repositoryLog.GetAllIncluding(x => x.LogActivityDetails).Where(x => x.LogActivityDetails.RelatedId == RelatedId).ToList();
        }
    }
}
