using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class MPMResponseDto
    {
        public bool status { get; set; }
        public string message { get; set; }
    }

    public class CustomerEngagementResponseDto : MPMResponseDto
    {
        public List<CustomerEngagementDataDto> data { get; set; }
    }

    public class EventMPMResposeDto : MPMResponseDto 
    {
        public List<MPMEventDto> data { get; set; }
    }

    public class LoginMPMResponseDto : MPMResponseDto 
    {
        public string key { get; set; }
    }
}
