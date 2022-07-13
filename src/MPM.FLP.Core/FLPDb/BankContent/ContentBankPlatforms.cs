using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ContentBankPlatforms : EntityBase
    {
        public string Name {get;set;}
        public string Description {get;set;}
    }
}
