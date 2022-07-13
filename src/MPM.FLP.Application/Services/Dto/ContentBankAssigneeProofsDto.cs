using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(ContentBankAssigneeProofs))]
    public class ContentBankAssigneeProofsCreateDto
    {
        public Guid GUIDContentBankAssignee { get; set; }
        public Guid GUIDContentBankPlatform { get; set; }
        public string Extension { get; set; }
        public string AttachmentURL { get; set; }
        public string RelatedLink { get; set; }
        public int ViewCount { get; set; }
        //public int ShareCount { get; set; }
        //public int LikeCount { get; set; }
        public DateTime UploadDate { get; set; }
        public string CreatorUsername { get; set; }
    }

    public class ContentBankAssigneeProofsUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDContentBankAssignee { get; set; }
        public Guid GUIDContentBankPlatform { get; set; }
        public long GUIDEmployee { get; set; }
        public string Extension { get; set; }
        public string AttachmentURL { get; set; }
        public string RelatedLink { get; set; }
        public int ViewCount { get; set; }
        //public int ShareCount { get; set; }
        //public int LikeCount { get; set; }
        public DateTime UploadDate { get; set; }
        public string LastModifierUsername { get; set; }
    }
}
