using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Web.Models.FLPMPM
{
    public class QuestionsVM
    {
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public string Question { get; set; }
        public string ImageUrl { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string OptionE { get; set; }
        public string CorrectOption { get; set; }
        public Guid ParentId { get; set; }
        public string Url { get; set; }
    }
}
