using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(ClaimProgramContents))]
    public class ClaimProgramContentsCreateDto
    {
        public Guid GUIDClaimProgramCampaign { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string KetentuanPoin { get; set; }
        public int Point { get; set; }
        public int MaximumClaim { get; set; }
        public bool IsOtp { get; set; }
        public bool IsH1 { get; set; }
        public bool IsH2 { get; set; }
        public bool IsH3 { get; set; }
        public bool IsTBSM { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreatorUsername { get; set; }
        public List<ClaimProgramContentAttachmentsDto> attachment { get; set; }
        //public List<ClaimProgramAssigneesDto> assignees { get; set; }
    }

    public class ClaimProgramContentsUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDClaimProgramCampaign { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string KetentuanPoin { get; set; }
        public int Point { get; set; }
        public int MaximumClaim { get; set; }
        public bool IsOtp { get; set; }
        public bool IsH1 { get; set; }
        public bool IsH2 { get; set; }
        public bool IsH3 { get; set; }
        public bool IsTBSM { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string LastModifierUsername { get; set; }
        public List<ClaimProgramContentAttachmentsDto> attachment { get; set; }
        //public List<ClaimProgramAssigneesDto> assignees { get; set; }
    }

    public class ClaimProgramContentAttachmentsUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDClaimProgramContent { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string AttachmentURL { get; set; }
    }
    public class ClaimProgramContentsDeleteDto
    {
        public Guid Id { get; set; }
        public string DeleterUsername { get; set; }
    }

    [AutoMapTo(typeof(ClaimProgramContentAttachments))]
    public class ClaimProgramContentAttachmentsDto
    {
        //public Guid GUIDClaimProgramContent { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string AttachmentURL { get; set; }
    }

    [AutoMapTo(typeof(ClaimProgramAssignees))]
    public class ClaimProgramAssigneesDto
    {
        public Guid GUIDClaimProgramContent { get; set; }
        public int IDJabatan { get; set; }
        public bool IsPassed { get; set; }
        public string Description { get; set; }
    }

    public class ClaimProgramContentDownloadDto
    {
        public string Username { get; set; }
        public string NamaFLP { get; set; }
        public string NoHP { get; set; }
        public string Jabatan { get; set; }
        public string NamaJaringan { get; set; }
        public string Kota { get; set; }
        public string Foto { get; set; }
    }

    public class ClaimProgramContentsListDto
    {
        public Guid Id { get; set; }
        public Guid GUIDClaimProgramCampaign { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string KetentuanPoin { get; set; }
        public int Point { get; set; }
        public int MaximumClaim { get; set; }
        public bool IsOtp { get; set; }
        public bool IsH1 { get; set; }
        public bool IsH2 { get; set; }
        public bool IsH3 { get; set; }
        public bool IsTBSM { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public List<ClaimProgramContentClaimersDto> claimer { get; set; }
        public List<ClaimProgramContentAttachments> attachment { get; set; }
    }

    public class ClaimProgramContentClaimersDto
    {
        public Guid Id { get; set; }
        //public Guid GUIDClaimProgramPrize { get; set; }
        //public Guid GUIDClaimProgramContent { get; set; }
        public int EmployeeId { get; set; }
        public string Username { get; set; }
        //public Guid GUIDClaimProgramPoint { get; set; }
        public string StorageUrl { get; set; }
        public string OTP { get; set; }
        public bool IsApprove { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedTime { get; set; }
        public int EnumStatus { get; set; }
       
    }
}
