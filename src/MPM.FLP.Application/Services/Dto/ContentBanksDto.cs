using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(ContentBanks))]
    public class ContentBanksCreateDto
    {
        public Guid GUIDContentBankCategory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Caption { get; set; }
        public int ReadingTime { get; set; }
        public bool H1 { get; set; }
        public bool H2 { get; set; }
        public bool H3 { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublished { get; set; }
        public string CreatorUsername { get; set; }
        public List<ContentBanksDetailsDto> Details { get; set; }
    }

    public class ContentBanksUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDContentBankCategory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Caption { get; set; }
        public int ReadingTime { get; set; }
        public bool H1 { get; set; }
        public bool H2 { get; set; }
        public bool H3 { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublished { get; set; }
        public string LastModifierUsername { get; set; }
    }

    public class ContentBanksDeleteDto
    {
        public Guid Id { get; set; }
        public string DeleterUsername { get; set; }
    }

    [AutoMapTo(typeof(ContentBankDetails))]
    public class ContentBanksDetailsDto
    {
        public string Name { get; set; }
        public int Orders { get; set; }
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Extension { get; set; }
        public string AttachmentURL { get; set; }
    }

    public class ContentBankResponse
    {
        public Guid Id { get; set; }
        public string AttachmentURL { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
