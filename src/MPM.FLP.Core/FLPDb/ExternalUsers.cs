using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ExternalUsers : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public long AbpUserId { get; set; }
        public string UserName { get; set; }
        public string Channel { get; set; }
        public string Name { get; set; }
        public string ShopName { get; set; }
        public string ShopImageurl { get; set; }
        public string KTPImageUrl { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Email { get; set; }
        public string Handphone { get; set; }
        public string Jabatan { get; set; }
        public bool IsKTPVerified { get; set; }
        public string UserImageUrl { get; set; }
        public string Kota { get; set; }
        public string CategoryH3 { get; set; }
        public string DealerCodeH3 { get; set; }

        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}
