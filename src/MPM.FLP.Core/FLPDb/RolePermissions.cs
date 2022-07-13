using Abp.Domain.Entities;
using System;

namespace MPM.FLP.FLPDb
{
    public class RolePermissions : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public Guid PermissionId { get; set; }
        public int RoleId { get; set; }
    }
}
