using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb
{
    public partial class SDMSLogs : Entity<Guid>
    {
        public SDMSLogs()
        {
        }

        public override Guid Id { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string EnumContentType {get;set;}
        public string EnumContentActivity {get;set;}
        public string EnumContentId {get;set;}
        public string EnumContentTitle {get;set;}
        public string Action {get;set;}
        public string MPMCode {get;set;}
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
    }

    public  class SDMSLogsDTO
    {
        public  string Id { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string EnumContentType {get;set;}
        public string EnumContentActivity {get;set;}
        public string EnumContentId {get;set;}
        public string EnumContentTitle {get;set;}
        public string Action {get;set;}
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string MDMCode {get;set;}
    }
}
