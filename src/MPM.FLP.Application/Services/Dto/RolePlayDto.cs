using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class RolePlayResultDto
    {
        public Guid? Id { get; set; }
        public int IDMPM { get; set; }
        public string NamaFLP { get; set; }
        public string KodeDealerMPM { get; set; }
        public string NamaDealerMPM { get; set; }
        public decimal? Result { get; set; }
        public string Grade { get; set; }
        public string StorageUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public Guid RolePlayId { get; set; }
        public List<RolePlayResultDetailDto> RolePlayResultDetailDto { get; set; }
    }

    public class RolePlayResultDetailDto 
    {
        public Guid? RolePlayDetailId { get; set; }
        public Guid RolePlayResultId { get; set; }
        public string Title { get; set; }
        public int? Order { get; set; }
        public bool? IsMandatorySilver { get; set; }
        public bool? IsMandatoryGold { get; set; }
        public bool? IsMandatoryPlatinum { get; set; }
        public bool? Passed { get; set; }
        public bool? NotPassed { get; set; }
        public bool? Dismiss { get; set; }
    }

    public class RolePlayMessageDto
    {
        public string Username { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
