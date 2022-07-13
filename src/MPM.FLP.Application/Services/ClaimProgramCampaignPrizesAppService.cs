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
    public class ClaimProgramCampaignPrizesAppService : FLPAppServiceBase, IClaimProgramCampaignPrizesAppService
    {
        private readonly IRepository<ClaimProgramCampaignPrizes, Guid> _repositoryPrizes;
        private readonly IRepository<ClaimProgramCampaigns, Guid> _repositoryCampaign;
        public ClaimProgramCampaignPrizesAppService(
            IRepository<ClaimProgramCampaignPrizes, Guid> repositoryPrizes,
            IRepository<ClaimProgramCampaigns, Guid> repositoryCampaign)
        {
            _repositoryPrizes = repositoryPrizes;
            _repositoryCampaign = repositoryCampaign;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            //var query = _repositoryPrizes.GetAll().Where(x => x.DeletionTime == null);
            var query = (from prizes in _repositoryPrizes.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           join campaign in _repositoryCampaign.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           on prizes.GUIDClaimProgramCampaign equals campaign.Id
                           select new ClaimProgramCampaignPrizesOutputDto
                           {
                               Id = prizes.Id,
                               GUIDClaimProgramCampaign = prizes.GUIDClaimProgramCampaign,
                               Name = prizes.Name,
                               Description = prizes.Description,
                               Prize = prizes.Prize,
                               RedeemPoint = prizes.RedeemPoint,
                               AttachmentURL = prizes.AttachmentURL,
                               EndDate = campaign.EndDate,
                               CreationTime = prizes.CreationTime,
                               CreatorUsername = prizes.CreatorUsername,
                               LastModificationTime = prizes.LastModificationTime,
                               LastModifierUsername = prizes.LastModifierUsername,
                               DeletionTime = prizes.DeletionTime,
                               DeleterUsername = prizes.DeleterUsername
                           }); 
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }
            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public ClaimProgramCampaignPrizes GetById(Guid Id)
        {
            var data = _repositoryPrizes.GetAll()
                        .FirstOrDefault(x => x.Id == Id);
            return data;
        }

        public void Create(ClaimProgramCampaignPrizesCreateDto input)
        {
            #region Create claim program campaign prizes
            var prizes = ObjectMapper.Map<ClaimProgramCampaignPrizes>(input);
            prizes.CreationTime = DateTime.Now;
            prizes.CreatorUsername = this.AbpSession.UserId.ToString();

            _repositoryPrizes.Insert(prizes);
            #endregion
        }
        public void Update(ClaimProgramCampaignPrizesUpdateDto input)
        {
            #region Update claim program campaign prizes
            var prizes = _repositoryPrizes.Get(input.Id);
            prizes.Name = input.Name;
            prizes.Description = input.Description;
            prizes.Prize = input.Prize;
            prizes.RedeemPoint = input.RedeemPoint;
            prizes.AttachmentURL = input.AttachmentURL;
            prizes.LastModifierUsername = this.AbpSession.UserId.ToString();
            prizes.LastModificationTime = DateTime.Now;

            _repositoryPrizes.Update(prizes);
            #endregion
        }
        public void SoftDelete(ClaimProgramCampaignPrizesDeleteDto input)
        {
            var prizes = _repositoryPrizes.Get(input.Id);
            prizes.DeleterUsername = this.AbpSession.UserId.ToString();
            prizes.DeletionTime = DateTime.Now;
            _repositoryPrizes.Update(prizes);
        }
    }
}
