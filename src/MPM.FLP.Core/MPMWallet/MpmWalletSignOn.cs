using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.MPMWallet
{
    public class MpmWalletSignOnRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Systrace { get; set; }
        public string Words { get; set; }


        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                { "clientId", ClientId },
                { "clientSecret", ClientSecret },
                { "systrace", Systrace },
                { "words", Words }
            };
            return dictionary;
        }
    }

    public class MpmWalletSignOnResponse : MpmWalletResponse
    {
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
