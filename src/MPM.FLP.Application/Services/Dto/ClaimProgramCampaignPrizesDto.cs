using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(ClaimProgramCampaignPrizes))]
    public class ClaimProgramCampaignPrizesCreateDto
    {
        public Guid GUIDClaimProgramCampaign { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Prize { get; set; }
        public int RedeemPoint { get; set; }
        public string AttachmentURL { get; set; }
    }

    public class ClaimProgramCampaignPrizesUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDClaimProgramCampaign { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Prize { get; set; }
        public int RedeemPoint { get; set; }
        public string AttachmentURL { get; set; }
    }

    public class ClaimProgramCampaignPrizesDeleteDto
    {
        public Guid Id { get; set; }
    }

    public class ClaimProgramCampaignPrizesOutputDto
    {
        public Guid Id { get; set; }
        public Guid GUIDClaimProgramCampaign { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Prize { get; set; }
        public int RedeemPoint { get; set; }
        public string AttachmentURL { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleterUsername { get; set; }
    }
}
