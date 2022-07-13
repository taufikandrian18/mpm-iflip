using Abp.Domain.Repositories;
using CorePush.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class BASTDetailsAppService : FLPAppServiceBase, IBASTDetailsAppService
    {
        private readonly IRepository<BASTDetails, Guid> _repositoryBASTDetail;
        public BASTDetailsAppService(
            IRepository<BASTDetails, Guid> repositoryBASTDetail)
        {
            _repositoryBASTDetail = repositoryBASTDetail;
        }
        public void Create(BASTDetailsDto input)
        {
            var detail = ObjectMapper.Map<BASTDetails>(input);
            detail.CreationTime = DateTime.Now;
            detail.CreatorUsername = this.AbpSession.UserId.ToString();
            Guid bastId = _repositoryBASTDetail.InsertAndGetId(detail);
        }
    }
}
