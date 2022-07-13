using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(ContentBankDetails))]
    public class ContentBanksDetailsUpdateDto
    {
        public Guid ContentBankId { get; set; }
        public List<ContentBanksDetailsDto> Details { get; set; }
        public string LastModifierUsername { get; set; }
    }

    public class ContentBanksDetailsByUserDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Caption { get; set; }
        public List<ContentBanksDetailAttachment> Details { get; set; }
    }

    public class ContentBanksDetailAttachment
    {
        public Guid Id { get; set; }
        public Guid GUIDContentBank { get; set; }
        public Guid GUIDContentBankAssignee { get; set; }
        public string Name { get; set; }
        public int Orders { get; set; }
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Extension { get; set; }
        public string AttachmentURL { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleterUsername { get; set; }
    }

    public class ContentBanksDetailsDeleteDto
    {
        public Guid Id { get; set; }
        public string DeleterUsername { get; set; }
    }
}
