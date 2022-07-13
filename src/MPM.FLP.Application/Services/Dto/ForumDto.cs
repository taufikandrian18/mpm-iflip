using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class ForumThreadCreateDto
    {
        public string Title { get; set; }
        public string Contents { get; set; }
        public string KodeDealerMPM { get; set; }
        public string Channel { get; set; }
        public string CreatorUsername { get; set; }
    }

    public class ForumPostCreateDto
    {
        public string Contents { get; set; }
        public Guid ForumThreadId { get; set; }
        public string CreatorUsername { get; set; }
    }
}
