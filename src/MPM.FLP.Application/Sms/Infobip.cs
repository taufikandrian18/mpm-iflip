using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Sms
{
    public class InfobipSms
    {
        public List<InfobipMessage> messages { get; set; }
    }

    public class InfobipMessage 
    {
        public string from { get { return AppConstants.InfobipSender; } }
        public List<InfobipDestination> destinations { get; set; }
        public string text { get; set; }
        public bool flash { get { return true; } }
    }
    public class InfobipDestination 
    {
        public string to { get; set; }
    }
}
