using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(ContentBankCategories))]
    public class ContentBankCategoriesCreateDto
    {
        public string Name { get; set; }
        public int Orders { get; set; }
        public string AttachmentUrl { get; set; }
        public string CreatorUsername { get; set; }
    }

    public class ContentBankCategoriesUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Orders { get; set; }
        public string AttachmentUrl { get; set; }
        public string LastModifierUsername { get; set; }
    }

    public class ContentBankCategoriesDeleteDto
    {
        public Guid Id { get; set; }
        public string DeleterUsername { get; set; }
    }
}
