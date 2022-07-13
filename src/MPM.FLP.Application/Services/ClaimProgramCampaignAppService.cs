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
    public class ClaimProgramCampaignAppService : FLPAppServiceBase, IClaimProgramCampaignAppService
    {
        private readonly IRepository<ClaimProgramCampaigns, Guid> _repositoryCampaign;
        private readonly IRepository<ClaimProgramCampaignPrizes, Guid> _repositoryPrizes;
        private readonly IRepository<ClaimProgramCampaignPoints, Guid> _repositoryPoint;
        private readonly IRepository<InternalUsers> _repositoryInternal;
        private readonly IRepository<ExternalUsers, Guid> _repositoryExternal;
        public ClaimProgramCampaignAppService(
            IRepository<ClaimProgramCampaigns, Guid> repositoryCampaign,
            IRepository<ClaimProgramCampaignPrizes, Guid> repositoryPrizes,
            IRepository<ClaimProgramCampaignPoints, Guid> repositoryPoint,
            IRepository<InternalUsers> repositoryInternal,
            IRepository<ExternalUsers, Guid> repositoryExternal)
        {
            _repositoryCampaign = repositoryCampaign; 
            _repositoryPrizes = repositoryPrizes;
            _repositoryPoint = repositoryPoint;
            _repositoryInternal = repositoryInternal;
            _repositoryExternal = repositoryExternal;
        }

        public BaseResponse GetAllAdmin([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryCampaign.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public BaseResponse GetActiveClaimProgram([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryCampaign.GetAll().Where(x => x.DeletionTime == null).OrderByDescending(x=> x.CreationTime).FirstOrDefault();
     

           
            var data = query;

            return BaseResponse.Ok(data, 1);
        }

        public ClaimProgramCampaigns GetById(Guid Id)
        {
            var data = _repositoryCampaign.GetAll()
                        .FirstOrDefault(x => x.Id == Id);
            return data;
        }

        public void Create(ClaimProgramCampaignsCreateDto input)
        {
            #region Create claim program campaign
            var campaign = ObjectMapper.Map<ClaimProgramCampaigns>(input);
            campaign.CreationTime = DateTime.Now;
            campaign.CreatorUsername = this.AbpSession.UserId.ToString();

            Guid campaignId = _repositoryCampaign.InsertAndGetId(campaign);
            #endregion

            #region Create prizes
            foreach (var prize in input.prizes)
            {
                var _prize = ObjectMapper.Map<ClaimProgramCampaignPrizes>(prize);
                _prize.GUIDClaimProgramCampaign = campaignId;
                _prize.CreatorUsername = this.AbpSession.UserId.ToString();
                _prize.CreationTime = DateTime.Now;
                _repositoryPrizes.Insert(_prize);
            }
            #endregion

            #region Create point all user
            var internalUser = _repositoryInternal.GetAll().Where(x => x.DeletionTime == null && x.AbpUserId != null).ToList();
            foreach (var inter in internalUser)
            {
                var dataPoint = new ClaimProgramCampaignPoints
                {
                    GUIDClaimProgramCampaign = campaignId,
                    EmployeeId = Convert.ToInt32(inter.AbpUserId),
                    Point = 0,
                    CreationTime = DateTime.Now,
                    CreatorUsername = this.AbpSession.UserId.ToString()
                };
                _repositoryPoint.InsertOrUpdate(dataPoint);
            }
            var externalUser = _repositoryExternal.GetAll().Where(x => x.DeletionTime == null && x.AbpUserId != null).ToList();
            foreach (var exter in externalUser)
            {
                var dataPoint = new ClaimProgramCampaignPoints
                {
                    GUIDClaimProgramCampaign = campaignId,
                    EmployeeId = Convert.ToInt32(exter.AbpUserId),
                    Point = 0,
                    CreationTime = DateTime.Now,
                    CreatorUsername = this.AbpSession.UserId.ToString()
                };
                _repositoryPoint.InsertOrUpdate(dataPoint);
            }
            #endregion
        }
        public void Update(ClaimProgramCampaignsUpdateDto input)
        {
            #region Update claim program campaign
            var campaign = _repositoryCampaign.Get(input.Id);
            campaign.Name = input.Name;
            campaign.Description = input.Description;
            campaign.IsActive = input.IsActive;
            campaign.LastModifierUsername = this.AbpSession.UserId.ToString();
            campaign.LastModificationTime = DateTime.Now;

            _repositoryCampaign.Update(campaign);
            #endregion

            #region Update prizes
            ////var prizes = _repositoryPrizes.GetAllList(x => x.GUIDClaimProgramCampaign == input.Id && x.DeletionTime == null);
            ////var i = 0;
            ////foreach (var prize in prizes)
            ////{
            ////    prize.Name = input.prizes[i].Name;
            ////    prize.Description = input.prizes[i].Description;
            ////    prize.Prize = input.prizes[i].Prize;
            ////    prize.RedeemPoint = input.prizes[i].RedeemPoint;
            ////    prize.AttachmentURL = input.prizes[i].AttachmentURL;
            ////    prize.LastModifierUsername = this.AbpSession.UserId.ToString();
            ////    prize.LastModificationTime = DateTime.Now;

            ////    _repositoryPrizes.Update(prize);
            ////    i++;
            ////}
            #endregion
        }
        public void SoftDelete(ClaimProgramCampaignsDeleteDto input)
        {
            var campaign = _repositoryCampaign.Get(input.Id);
            campaign.DeleterUsername = this.AbpSession.UserId.ToString();
            campaign.DeletionTime = DateTime.Now;
            _repositoryCampaign.Update(campaign);

            SoftDeletePrizes(input.Id);
        }
        private void SoftDeletePrizes(Guid CampaignId)
        {
            var prizes = _repositoryPrizes.GetAllList(x => x.GUIDClaimProgramCampaign == CampaignId && x.DeletionTime == null);
            foreach (var prize in prizes)
            {
                prize.DeleterUsername = this.AbpSession.UserId.ToString();
                prize.DeletionTime = DateTime.Now;
                _repositoryPrizes.Update(prize);
            }
        }
    }
}
