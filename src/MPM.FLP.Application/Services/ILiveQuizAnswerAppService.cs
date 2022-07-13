﻿using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ILiveQuizAnswerAppService : IApplicationService
    {
        IQueryable<LiveQuizAnswers> GetAll();
        List<LiveQuizAnswers> GetByLiveQuizHistory(Guid LiveQuizHistoryId);
    }
}
