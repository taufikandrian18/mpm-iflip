using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class RolePlayAssignments : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public Guid RolePlayId { get; set; }
        public string KodeDealerMPM { get; set; }
        public string NamaDealer { get; set; }

        public virtual RolePlays RolePlay { get; set; }
    }
}
