using Abp;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MPM.FLP.LogActivity
{
    public interface ILogActivityAppService
    {
        void CreateLogActivity(long UserId, string UserName, string PageName, Guid RelatedId, string RelatedTitle, string Action, Object OldObject, Object NewObject);
        List<LogActivities> GetByRelatedId(Guid RelatedId);
    }
}
