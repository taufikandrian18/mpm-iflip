using CorePush.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using MPM.FLP.Configuration;
using MPM.FLP.PushNotification;
using MPM.FLP.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MPM.FLP
{
    public class AppConstants
    {
        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public const string DefaultPassPhrase = "gsKxGZ012HLL3MI5";

        //Firebase using Altrovis Account
        //public const string ServerKey = "AAAAvEiSF78:APA91bHdU5fQ-dMHCTEfPaxnE1mgl8n6UD0-v_qsKJXB-5JS3ZJyOCh41NiMNENT9Ici8AMGM2aTMWTSxqT2WRePGyV41I_6bJAiv-7UAz6_4gGk5xl4ZS9Y7XrW0vWwXCVQfzcrEhxN";
        //public const string SenderID = "AIzaSyDuxjRDVH8ay8UcQKtVHpOB_gInV1Lwqis";

        //Firebase using MPM Account
        public const string ServerKey = "AAAATleR2lU:APA91bE4oJnyqXGGphJuP5H8OMQUAdfCV3PHOCtPHSY6SJrNWQQnvGau-2RjvsLzo3BsBK20I47eiUpX9AmTqJSEyu3PSSSbmH4bM9nt-LdZVBhFE4TZBU7cmFQ61J9YY_g6QJAKIlbm";
        public const string SenderID = "336476625493";

        //Zensiva SMS 
        public const string SmsApiUrl = "https://reguler.zenziva.net/apps/smsapi.php";
        public const string UserKey = "7fkjwd";
        public const string PassKey = "4dm1n@mpm";

        //Infobip
        public const string InfobipAPIKeyAuthorization = "159fd662106cbf9c0be2007a31e92894-40e10695-ac70-480d-a7fc-c9214ade3d01";
        public const string InfobipBase64Auhtorization = "bXBtX21vdG9yMTpjZGJoYzM=";
        public const string InfobipSender = "HONDA JATIM";
        public const string InfobipUrl = "https://6dnv5.api.infobip.com/sms/2/text/advanced?async=1";

        // MPM API
        public const string MpmLoginUrl = "https://api.mpm-motor.com/mpm/token/api/1.0/login";
        public const string MpmLoginUsername = "altrovis";
        public const string MpmLoginPassword = "altrovis2019!";
        public const string MpmCustomerUrl = "https://api.mpm-motor.com/marketingv2/flp/getcustomer";
        public const string MPMEventUrl = "https://api.mpm-motor.com/marketingv2/flp/getlistevent";
        public const string MPMDealerUrl = "https://api.mpm-motor.com/marketingv2/flp/getdealer?CHANNEL={0}";

        public const string MPMProductUrl = "https://vps.mpm-motor.com/marketingv3/integrationsdms/getmasterunit?itemgroupname={0}";
        public const string MPMPenjualanUrl = "https://vps.mpm-motor.com/marketingv3/integrationsdms/getpenjualanunit?datefrom={0}&dateto={1}&kodeDealer={2}";
    }

    public class AppHelpers
    {
        public static async Task<string> MPMLogin()
        {
            HttpClient client = new HttpClient();
            IList<KeyValuePair<string, string>> loginParameters = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("username", AppConstants.MpmLoginUsername) },
                { new KeyValuePair<string, string>("password", AppConstants.MpmLoginPassword) }
            };

            var loginResult = await client.PostAsync(AppConstants.MpmLoginUrl, new FormUrlEncodedContent(loginParameters));
            var loginJson = await loginResult.Content.ReadAsStringAsync();

            LoginMPMResponseDto loginResponse = JsonConvert.DeserializeObject<LoginMPMResponseDto>(loginJson);
            var key = loginResponse.key;

            client.Dispose();

            return key;
        }

        public static IConfigurationRoot GetConnectionToAzure()
        {
            //For publish connection
            return AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            //For local connection
            //return AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());
        }

        public static async Task<string> InsertAndGetUrlAzure(IFormFile file, string nama, string container)
        {
            string url = "";

            var configuration = GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(container);

                var path = Path.GetExtension(file.FileName);

                string namaFile = nama + path;

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(namaFile);


                //await cloudBlockBlob.UploadFromFileAsync(file.FileName);
                using (Stream stream = file.OpenReadStream())
                {
                    stream.Position = 0;
                    await cloudBlockBlob.UploadFromStreamAsync(stream);
                }


                url = cloudBlockBlob.Uri.AbsoluteUri;
            }

            return url;
        }
        public static async Task SendPushNotification(string deviceToken, GoogleNotification notification) 
        {
            using (var fcm = new FcmSender(AppConstants.ServerKey, AppConstants.SenderID))
            {
                await fcm.SendAsync(deviceToken, notification);
            }
        }

        public static GoogleNotification CreateNotification(string content)
        {
            return new GoogleNotification()
            {
                Data = new GoogleNotification.DataPayload()
                {
                    Message = content,
                
                }
            };
            
        }

        public static GoogleNotification CreateNotification(object content)
        {
            return new GoogleNotification()
            {
                Data = new GoogleNotification.DataPayload()
                {
                    Message = JsonConvert.SerializeObject(content),
                
                }
            };
            
        }

    }
}
