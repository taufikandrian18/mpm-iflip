using Abp.Dependency;
using MPM.FLP.MPMWallet;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MPM.FLP.Doku
{
    public class DokuSettings : MpmWalletSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SharedKey { get; set; }
        public static string Systrace { get; set; }
        public DokuWebServiceUrl WebServiceUrl { get; set; }
        public static string AccessToken { get; set; }

        public string GetHash(string input)
        {
            byte[] byteKey = Encoding.UTF8.GetBytes(ClientSecret ?? "");
            byte[] byteInput = Encoding.UTF8.GetBytes(input ?? "");
            using (HMACSHA1 sha1 = new HMACSHA1(byteKey))
            {
                byte[] byteResult = sha1.ComputeHash(byteInput);
                StringBuilder strResult = new StringBuilder();
                for (int i = 0; i < byteResult.Length; i++)
                {
                    strResult.Append(byteResult[i].ToString("x2"));
                }
                return strResult.ToString();
            }
        }
    }

    public class DokuWebServiceUrl
    {
        public string SignOn { get; set; }
        public string Register { get; set; }
        public string NewRegister { get; set; }
        public string Balance { get; set; }
        public string Histories { get; set; }
    }
}
