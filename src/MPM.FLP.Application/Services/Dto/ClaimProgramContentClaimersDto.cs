using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(ClaimProgramContentClaimers))]
    public class ClaimProgramContentClaimersCreateDto
    {
        //public Guid GUIDClaimProgramPrize { get; set; }
        public Guid GUIDClaimProgramContent { get; set; }
        //public int EmployeeId { get; set; }
        //public Guid GUIDClaimProgramPoint { get; set; }
        public string StorageUrl { get; set; }
        //public string OTP { get; set; }
        //public bool IsApprove { get; set; }
        //public bool IsVerified { get; set; }
        //public DateTime? VerifiedTime { get; set; }
    }

    public class ClaimProgramContentClaimersUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDClaimProgramPrize { get; set; }
        public Guid GUIDClaimProgramContent { get; set; }
        public int EmployeeId { get; set; }
        public Guid GUIDClaimProgramPoint { get; set; }
        public string StorageUrl { get; set; }
        public string OTP { get; set; }
        public int EnumStatus { get; set; }
        //public bool IsApprove { get; set; }
        //public bool IsVerified { get; set; }
        //public DateTime? VerifiedTime { get; set; }
    }

    public class ClaimProgramContentClaimersDeleteDto
    {
        public Guid Id { get; set; }
    }
    public class ApprovalContentClaimerDto
    {
        public Guid ClaimProgramContentClaimerId { get; set; }
        public bool IsApprove { get; set; }
    }
    public class VerifyClaimerDto
    {
        public Guid ClaimProgramContentClaimerId { get; set; }
        public string OTP { get; set; }
    }
    public class FilterGetClaimerDto
    {
        public Guid ClaimProgramContentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class RedeemPrizeClaimerDto
    {
        public Guid ClaimProgramCampaignPrizesId { get; set; }
        //public Guid ClaimProgramCampaignPointId { get; set; }
    }
}
