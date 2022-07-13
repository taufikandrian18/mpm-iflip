using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using CorePush.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class SDMSMessageWebService : FLPAppServiceBase, ISDMSMesssageWeb
    {
        private readonly IRepository<SDMSMessageWeb, Guid> _sdms;
        private readonly IRepository<SDMSMessageDetail, Guid> _sdmsDetail;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;

        public SDMSMessageWebService(IRepository<SDMSMessageWeb, Guid> sdmsRepo, IRepository<SDMSMessageDetail, Guid> sdmsDetailRepo, IRepository<InternalUsers> internalUserRepository, IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository)
        {
            _sdms = sdmsRepo;
            _sdmsDetail = sdmsDetailRepo;
            _internalUserRepository = internalUserRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
        }

        public IList<SDMSMessageWeb> GetAll()
        {
            return _sdms.GetAll().Where(x => !x.DeletionTime.HasValue).ToList();
        }

        public IList<SDMSMessageVM> GetSentBox(string userId, int limit, int page)
        {
            if (limit == 0 || page == 0)
            {
                limit = 1;
                page = 1;
            }
            page = (page - 1) * limit;

            var data = _sdms.GetAll().Where(x => x.SenderId == userId && !x.DeletionTime.HasValue).OrderByDescending(x => x.CreationTime).Skip(page).Take(limit).Select(
                x => new SDMSMessageVM
                {
                    Body = x.Body,
                    Subject = x.Subject,
                    LastModificationTime = x.LastModificationTime,
                    Id = x.Id,
                    SenderId = x.SenderId,
                    SenderUsername = x.SenderUsername,
                    ReadStatus = x.SDMSMessageDetail.FirstOrDefault(z => z.RecipientId == userId).ReadStatus,
                    CreationTime = x.CreationTime
                }).ToList();
            return data;
        }

        public IList<SDMSMessageVM> GetInbox(string userId, int limit, int page)
        {
            if (limit == 0 || page == 0)
            {
                limit = 1;
                page = 1;
            }

            page = (page - 1) * limit;
            var data = _sdms.GetAll().Include(x => x.SDMSMessageDetail).Where(x => x.SDMSMessageDetail.Where(y => y.RecipientId == userId).Any() && !x.DeletionTime.HasValue).OrderByDescending(x => x.CreationTime).Skip(page).Take(limit).Select(
                x => new SDMSMessageVM
                {
                    Body = x.Body,
                    Subject = x.Subject,
                    LastModificationTime = x.LastModificationTime,
                    Id = x.Id,
                    SenderId = x.SenderId,
                    SenderUsername = x.SenderUsername,
                    ReadStatus = x.SDMSMessageDetail.FirstOrDefault(z => z.RecipientId == userId).ReadStatus,
                    CreationTime = x.CreationTime
                }).ToList();
            return data;
        }

        public SDMSMessageWeb GetDetail(Guid id)
        {
            return _sdms.GetAll().Include(x => x.SDMSMessageDetail).FirstOrDefault(x => x.Id.ToString().ToLower() == id.ToString().ToLower());
        }

        [HttpPost]
        public void Create(SDMSMessageWeb input)
        {
            try {
            int id = 0;
            var mpmUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.Nama.ToLower().Replace(" ", "") == input.SenderUsername.ToLower().Replace(" ", ""));
            if (mpmUser != null)
            {
                id = mpmUser.IDMPM;
            }

            input.CreationTime = DateTime.UtcNow.AddHours(7);
            input.CreatorUsername = input.SenderUsername;
            input.Id = Guid.NewGuid();

            foreach (SDMSMessageWebDetail detail in input.SDMSMessageDetail)
            {
                detail.MessageId = input.Id;
                detail.Id = Guid.NewGuid();
                detail.CreationTime = DateTime.UtcNow.AddHours(7);
                detail.CreatorUsername = input.SenderUsername;
            }


            _sdms.Insert(input);
            SendNewMessage(input);
        } catch(Exception x){
            Console.WriteLine(x.Message);
        }

        }

        [HttpDelete]
        public void SoftDelete(Guid id)
        {
            var data = _sdms.FirstOrDefault(x => x.Id == id);
            data.DeleterUsername = this.AbpSession.UserId.ToString();
            data.DeletionTime = DateTime.Now;
            _sdms.Update(data);

            var detail = _sdmsDetail.GetAll().Where(x => x.MessageId == id).ToList();
            foreach (SDMSMessageDetail details in detail)
            {
                details.DeletionTime = DateTime.Now;
                data.DeleterUsername = this.AbpSession.UserId.ToString();
                _sdmsDetail.Update(details);
            }
        }

        [HttpPut]
        public void Read(Guid messageID, string readerID)
        {
            var data = _sdmsDetail.FirstOrDefault(x => x.MessageId == messageID && x.RecipientId == readerID);
            data.LastModificationTime = DateTime.Now;
            data.ReadStatus = 1;
            data.LastModifierUsername = this.AbpSession.UserId.ToString();
            _sdmsDetail.Update(data);
        }

        async Task SendNewMessage(SDMSMessageWeb message)
        {
            List<string> deviceTokens = new List<string>();
            List<string> users = message.SDMSMessageDetail.Select(x => x.RecipientId).ToList();
            deviceTokens.AddRange
            ((
                from p in _pushNotificationSubscriberRepository.GetAll()
                join i in _internalUserRepository.GetAll()
                on p.Username equals i.IDMPM.ToString()
                where users.Contains(i.IDMPM.ToString())
                select p.DeviceToken
             ).ToList());

           /* var data = new MessageBody{
                Category = "SDMSMessage",
                Id = message.Id.ToString(),
                SenderUserName = message.SenderUsername,
            };*/

            var data = "SDMSMESSAGE," + message.Id + "," + message.SenderUsername+","+message.Body;

            
            foreach (var deviceToken in deviceTokens)
            {
                using (var fcm = new FcmSender(AppConstants.ServerKey, AppConstants.SenderID))
                {
                    var notification = AppHelpers.CreateNotification(data);
                    await fcm.SendAsync(deviceToken, notification);
                }
            }
        }

        public class MessageBody{
            public string Category {get;set;}
            public string Id {get;set;}
            public string SenderUserName {get;set;}
        }

    }
}
