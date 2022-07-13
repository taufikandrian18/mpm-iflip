using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class HomeworkQuizHistoryCreateDto
    {
        public Guid HomeworkQuizId { get; set; }
        public int? IDMPM { get; set; }
        public string Name { get; set; }
        public string Jabatan { get; set; }
        public string Kota { get; set; }
        public string Dealer { get; set; }
        public int? CorrectAnswer { get; set; }
        public int? WrongAnswer { get; set; }
        public decimal? Score { get; set; }
        public string CreatorUsername { get; set; }

        public List<HomeworkQuizAnswerCreaterDto> HomewrorkAnswer { get; set; }
    }

    public class HomeworkQuizAnswerCreaterDto 
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
