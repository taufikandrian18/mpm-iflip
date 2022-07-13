//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MPM.FLP
//{
//    public class MpmWalletSignOnRequest
//    {
//        public string ClientId { get; set; }
//        public string ClientSecret { get; set; }
//        public string Systrace { get; set; }
//        public string Words { get; set; }

//        public Dictionary<string, string> ToDictionary()
//        {
//            Dictionary<string, string> dictionary = new Dictionary<string, string>();
//            dictionary.Add("clientId", ClientId);
//            dictionary.Add("clientSecret", ClientSecret);
//            dictionary.Add("systrace", Systrace);
//            dictionary.Add("words", Words);
//            return dictionary;
//        }
//    }

//    public class MpmWalletSignOnResponse
//    {
//        public string ClientId { get; set; }
//        public string ResponseCode { get; set; }
//        public MpmWalletSignOnResponseMessage ResponseMessage { get; set; }
//        public string AccessToken { get; set; }
//        public int ExpiresIn { get; set; }
//    }

//    //public class MpmWalletSignOnResponseMessage
//    //{
//    //    public string Id { get; set; }
//    //    public string En { get; set; }
//    //}
//}
