using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class NewClaimerDto
    {
        public string ClaimerUsername { get; set; }
        public IFormFile file { get; set; }
        public Guid ClaimProgramId { get; set; }
    }

    public class UpdateClaimerDto
    {
        public IFormFile file { get; set; }
        public Guid ClaimProgramClaimerId { get; set; }
    }

    public class ClaimerDto 
    {
        public Guid ClaimProgramClaimerId { get; set; }
        public string ClaimerUsername { get; set; }
        public string NamaUser { get; set; }
        public string StorageUrl { get; set; }
        public Guid ClaimProgramId { get; set; }
        public bool? IsApproved { get; set; }
        public int? EnumStatus { get; set; }
        public string OTP { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedTime { get; set; }
        public string ShopName { get; set; }
        public string ShopImageurl { get; set; }
        public string KTPImageUrl { get; set; }
        public string Address { get; set; }
        //public string Longitude { get; set; }
        //public string Latitude { get; set; }
        public string Email { get; set; }
        public string Handphone { get; set; }
        public string Jabatan { get; set; }
        public bool IsKTPVerified { get; set; }
        public string UserImageUrl { get; set; }
        public string KodeDealer { get; set; }
        public string NamaDealer { get; set; }
        public string KotaDealer { get; set; }
        //public int? IDFLP{ get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ApprovalClaimDto 
    {
        public Guid ClaimProgramClaimerId { get; set; }
        public bool IsApproved { get; set; }
    }

    public class VerifyClaimDto 
    {
        public Guid ClaimProgramClaimerId { get; set; }
        public string OTP { get; set; }
    }

    public class NotifikasiClaimerDto 
    {
        public Guid ClaimProgramId { get; set; }
        public string username { get; set; }
        public bool IsApproved { get; set; }
    }

    public class ClaimerWebNotificationDto 
    {
        public Guid ClaimProgramId { get; set; }
        public Guid ClaimProgramClaimerId { get; set; }
        public string CreatorUsername { get; set; }
        public bool IsReclaim { get; set; }
    }

    public class ListClaimerDto
    {
        public Guid ClaimProgramClaimerId { get; set; }
        public bool? IsApproved { get; set; }
        public int EnumStatus { get; set; }
        public string ClaimProgramContentName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Point { get; set; }
        public int EmployeeId { get; set; }
        public string StorageUrl { get; set; }
        public bool IsH1 { get; set; }
        public bool IsH2 { get; set; }
        public bool IsH3 { get; set; }
        public bool IsTbsm { get; set; }
    }
}
