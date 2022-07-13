using Microsoft.Extensions.Options;
using MPM.FLP.MPMWallet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Doku.Dto
{
    public class DokuSignOnRequestDto
    {
        public DokuSignOnRequestDto(DokuSettings dokuSettings)
        {
            ClientId = dokuSettings.ClientId;
            ClientSecret = dokuSettings.ClientSecret;
            Systrace = DokuSettings.Systrace;
            Words = dokuSettings.GetHash(dokuSettings.ClientId + dokuSettings.SharedKey + DokuSettings.Systrace);
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Systrace { get; set; }
        public string Words { get; set; }

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("clientId", ClientId);
            dictionary.Add("clientSecret", ClientSecret);
            dictionary.Add("systrace", Systrace);
            dictionary.Add("words", Words);
            return dictionary;
        }
    }


    public class DokuSignOnResponse : MpmWalletSignOnResponse
    {
        //public string ClientId { get; set; }
        //public string ResponseCode { get; set; }
        //public MpmWalletResponseMessage ResponseMessage { get; set; }
        //public string AccessToken { get; set; }
        //public int ExpiresIn { get; set; }
    }
}
