using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class SelfRecordingResultDto
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
        public Guid SelfRecordingId { get; set; }
        public List<SelfRecordingResultDetailDto> SelfRecordingResultDetailDto { get; set; }
    }

    public class SelfRecordingResultDetailDto
    {
        public Guid? SelfRecordingDetailId { get; set; }
        public string Title { get; set; }
        public int? Order { get; set; }
        public bool? IsMandatorySilver { get; set; }
        public bool? IsMandatoryGold { get; set; }
        public bool? IsMandatoryPlatinum { get; set; }
        public bool? Passed { get; set; }
        public bool? NotPassed { get; set; }
        public bool? Dismiss { get; set; }
    }

    public class SelfRecordingMessageDto
    {
        public string Username { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
