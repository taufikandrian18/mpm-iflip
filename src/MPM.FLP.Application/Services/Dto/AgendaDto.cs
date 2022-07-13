using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class AgendaCreateDto
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ComingDate { get; set; }
        //public string StorageUrl { get; set; }
        public IFormFile file { get; set; }
        public string CreatorUsername { get; set; }
        public List<string> Assignments { get; set; }

    }

    public class AgendaAssignDto
    {
        public Guid AgendaId { get; set; }
        public string CreatorUsername { get; set; }
        public List<int> Assignments { get; set; }
    }

    public class AgendaDeleteDto
    {
        public Guid AgendaId { get; set; }
        public List<int> IDMPM { get; set; }
    }
}
