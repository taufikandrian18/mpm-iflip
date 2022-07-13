using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Web.Models.ProgramManagement
{
    public class ProgramManagementViewModel
    {
        public Guid Id { get; set; }
        public DateTime Tanggal { get; set; }
        public bool IsPublished { get; set; }
        public string FeaturedImage { get; set; }
        public string Judul { get; set; }
        public string Contents { get; set; }
        public string Author { get; set; }
    }
}
