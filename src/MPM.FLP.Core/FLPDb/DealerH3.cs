using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MPM.FLP.FLPDb
{
    public partial class DealerH3 : Entity<string>
    {
        public DealerH3()
        {
            //ForumThreads = new HashSet<ForumThreads>();
            //ProductPrices = new HashSet<ProductPrices>();
        }
        
        [NotMapped]
        public override string Id { get { return AccountNumber; } }
        [Key]
        public string AccountNumber { get; set; }
        public string KodeDealerAHM { get; set; }
        
        public string KodeDealerMPM { get; set; }
        public string Nama { get; set; }
        public string Alamat { get; set; }
        public string Kota { get; set; }
        public string Channel { get; set; }
        public string EmailDealer { get; set; }
        public string KodeKareswil { get; set; }
        public string Karesidenan { get; set; }
        public int? NPKSupervisor { get; set; }
        public string NamaSupervisor { get; set; }
        public string EmailSupervisor { get; set; }
        public string IdKaresidenanHC3 { get; set; }
        public string NamaKaresidenanHC3 { get; set; }
        public int? NPKSupervisorHC3 { get; set; }
        public string NamaSupervisorHC3 { get; set; }
        public int? NPKDepartmentHeadHC3 { get; set; }
        public string NamaDepartmentHeadHC3 { get; set; }
        public int? NPKDivisionHeadHC3 { get; set; }
        public string NamaDivisionHeadHC3 { get; set; }
        public string IdKaresidenanTSD { get; set; }
        public string NamaKaresidenanTSD { get; set; }
        public int? NPKSupervisorTSD { get; set; }
        public string NamaSupervisorTSD { get; set; }
        public int? NPKDepartmentHeadTSD { get; set; }
        public string NamaDepartmentHeadTSD { get; set; }
        public int? NPKDivisionHeadTSD { get; set; }
        public string NamaDivisionHeadTSD { get; set; }

        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }

        //[JsonIgnore]
        //public virtual ICollection<ForumThreads> ForumThreads { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<ProductPrices> ProductPrices { get; set; }
    }
}
