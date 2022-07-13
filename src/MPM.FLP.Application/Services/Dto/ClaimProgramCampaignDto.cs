using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(ClaimProgramCampaigns))]
    public class ClaimProgramCampaignsCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<ClaimProgramCampaignPrizesDto> prizes { get; set; }
    }

    public class ClaimProgramCampaignsUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public List<ClaimProgramCampaignPrizesDto> prizes { get; set; }
    }

    public class ClaimProgramCampaignsPrizesUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDClaimProgramCampaign { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Prize { get; set; }
        public int RedeemPoint { get; set; }
        public string AttachmentURL { get; set; }
    }
    public class ClaimProgramCampaignsDeleteDto
    {
        public Guid Id { get; set; }
    }

    [AutoMapTo(typeof(ClaimProgramCampaignPrizes))]
    public class ClaimProgramCampaignPrizesDto
    {
        public Guid GUIDClaimProgramCampaign { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Prize { get; set; }
        public int RedeemPoint { get; set; }
        public string AttachmentURL { get; set; }
    }

}
