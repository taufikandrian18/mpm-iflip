using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using MPM.FLP.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class ContentBankDownloadDto
    {
        public string Karesidenan { get; set; }
        public string KodeDealerAHM { get; set; }
        public string KodeDealerMPM { get; set; }
        public string NamaDealer { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        
    }

    public class ContentBankDownloadDetailDto
    {
        public string Content { get; set; }
        public int Status { get; set; }

    }
    public class ContentBankSosmedDto
    {
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string KodeDealerMPM { get; set; }
        public string KodeDealerAHM { get; set; }
        public string NamaDealer { get; set; }
        public string Kota { get; set; }
        public string Channel { get; set; }
        public string Karesidenan { get; set; }
        public Nullable<DateTime> DownloadDate { get; set; }
        public string UploadWa { get; set; }
        public int TotalViewWa { get; set; }
        public Nullable<DateTime> UploadDateFb { get; set; }
        public string LinkFb { get; set; }
        public int TotalViewFb { get; set; }
        public Nullable<DateTime> UploadDateIg { get; set; }
        public string LinkIg { get; set; }
        public int TotalViewIg { get; set; }
        public int Status { get; set; }
    }
}
