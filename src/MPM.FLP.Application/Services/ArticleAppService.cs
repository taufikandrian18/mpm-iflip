using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using CorePush.Google;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using Newtonsoft.Json;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ArticleAppService : FLPAppServiceBase, IArticleAppService
    {
        private readonly IRepository<Articles, Guid> _articleRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;
        public ArticleAppService(IRepository<Articles, Guid> articleRepository,
                                 IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
                                 IRepository<InternalUsers> internalUserRepository,
                                 IRepository<ExternalUsers, Guid> externalUserRepository,
                                 IAbpSession abpSession,
                                 LogActivityAppService logActivityAppService)
        {
            _articleRepository = articleRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _internalUserRepository = internalUserRepository;
            _externalUserRepository = externalUserRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<Articles> GetAll()
        {
            return _articleRepository.GetAll().Where(x => x.DeletionTime == null).Include(y => y.ArticleAttachments);
        }

        public IQueryable<Articles> GetAllList()
        {
            return _articleRepository.GetAll().Where(x=> x.DeletionTime == null);
        }

        public ICollection<ArticleAttachments> GetAllAttachments(Guid id)
        {
            var articles = _articleRepository.GetAll().Include(x => x.ArticleAttachments);
            var attachments = articles.FirstOrDefault(x => x.Id == id).ArticleAttachments;
            return attachments;
        }

        public List<Guid> GetAllIds(string channel)
        {
            var article = _articleRepository.GetAll().Where(x => x.IsPublished
                                                              && string.IsNullOrEmpty(x.DeleterUsername))
                                                     .OrderByDescending(x => x.CreationTime);
            switch (channel)
            {
                case "H1":
                    return article.Where(x => x.H1).Select(x => x.Id).ToList();
                case "H2":
                    return article.Where(x => x.H2).Select(x => x.Id).ToList();
                case "H3":
                    return article.Where(x => x.H3).Select(x => x.Id).ToList();
                default:
                    return article.Select(x => x.Id).ToList();
            }
        }

        public List<Guid> GetAllH1Ids(string channel)
        {
            var article = _articleRepository.GetAll().Where(x => x.Resource == "H1" 
                                                              && x.IsPublished
                                                              && string.IsNullOrEmpty(x.DeleterUsername))
                                                     .OrderByDescending(x => x.CreationTime);
            switch (channel)
            {
                case "H1":
                    return article.Select(x => x.Id).ToList();
                case "H2":
                    return article.Where(x => x.H2).Select(x => x.Id).ToList();
                case "H3":
                    return article.Where(x => x.H3).Select(x => x.Id).ToList();
                default:
                    return article.Select(x => x.Id).ToList();
            }
        }

        public List<Guid> GetAllH2Ids(string channel)
        {
            var article = _articleRepository.GetAll().Where(x => x.Resource == "H2"
                                                              && x.IsPublished
                                                              && string.IsNullOrEmpty(x.DeleterUsername))
                                                     .OrderByDescending(x => x.CreationTime);
            switch (channel)
            {
                case "H1":
                    return article.Where(x => x.H1).Select(x => x.Id).ToList();
                case "H2":
                    return article.Select(x => x.Id).ToList();
                case "H3":
                    return article.Where(x => x.H3).Select(x => x.Id).ToList();
                default:
                    return article.Select(x => x.Id).ToList();
            }
        }

        public List<Guid> GetAllH3Ids(string channel)
        {
            var article = _articleRepository.GetAll().Where(x => x.Resource == "H3"
                                                             && x.IsPublished
                                                             && string.IsNullOrEmpty(x.DeleterUsername))
                                                     .OrderByDescending(x => x.CreationTime);
            switch (channel)
            {
                case "H1":
                    return article.Where(x => x.H1).Select(x => x.Id).ToList();
                case "H2":
                    return article.Where(x => x.H2).Select(x => x.Id).ToList();
                case "H3":
                    return article.Select(x => x.Id).ToList();
                default:
                    return article.Select(x => x.Id).ToList();
            }
        }

        public List<Guid> GetAllHC3Ids(string channel)
        {
            var article = _articleRepository.GetAll().Where(x => x.Resource == "HC3"
                                                             && x.IsPublished
                                                             && string.IsNullOrEmpty(x.DeleterUsername))
                                                     .OrderByDescending(x => x.CreationTime);
            switch (channel)
            {
                case "H1":
                    return article.Where(x => x.H1).Select(x => x.Id).ToList();
                case "H2":
                    return article.Where(x => x.H2).Select(x => x.Id).ToList();
                case "H3":
                    return article.Where(x => x.H3).Select(x => x.Id).ToList();
                default:
                    return article.Select(x => x.Id).ToList();
            }
        }

        public Articles GetById(Guid id)
        {
            var article = _articleRepository.GetAll().Include(x => x.ArticleAttachments).FirstOrDefault(x => x.Id == id);
            return article;
        }

        public void Create(Articles input)
        {
            input.Id = Guid.NewGuid();
            input.CreationTime = DateTime.UtcNow.AddHours(7);
            var id = AbpSession.UserId.ToString();

            if (!input.CreatorUsername.Any())
            {
                input.CreatorUsername = input.CreatorUsername;
            }

            input.DeleterUsername = null;

            if (input.Resource.IsNullOrEmpty()) {
                input.Resource = "";
            }

            foreach (ArticleAttachments attachments in input.ArticleAttachments)
            {
                attachments.ArticleId = input.Id;
                attachments.CreationTime = input.CreationTime;
                attachments.CreatorUsername = input.CreatorUsername;
                attachments.Id = Guid.NewGuid();
            }

            //_articleRepository.Insert(input);
            var articleId = _articleRepository.InsertAndGetId(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Article", articleId, input.Title, LogAction.Create.ToString(), null, input);
            SendArticleNotification(input);
        }

        public void Update(Articles input)
        {
            /*
            foreach (ArticleAttachments attachments in input.ArticleAttachments)
            {
                attachments.ArticleId = input.Id;
                attachments.CreationTime = input.CreationTime;
                attachments.CreatorUsername = input.CreatorUsername;
            }
            */
            var oldObject = _articleRepository.GetAll().AsNoTracking().Include(x => x.ArticleAttachments).FirstOrDefault(x => x.Id == input.Id);
            _articleRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Article", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);

        }

        public void SoftDelete(Guid id, string username)
        {
            var article = _articleRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _articleRepository.GetAll().AsNoTracking().Include(x => x.ArticleAttachments).FirstOrDefault(x => x.Id == id);
            article.DeleterUsername = username;
            article.DeletionTime = DateTime.Now;
            _articleRepository.Update(article);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Article", id, article.Title, LogAction.Delete.ToString(), oldObject, article);

        }

        async Task SendArticleNotification(Articles article)
        {
            List<string> deviceTokens = new List<string>();

            if (article.H1)
            {
                deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join i in _internalUserRepository.GetAll()
                    on p.Username equals i.IDMPM.ToString()
                    where i.Channel == "H1"
                    select p.DeviceToken
                 ).ToList());
            }

            if (article.H2)
            {
                deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join i in _internalUserRepository.GetAll()
                    on p.Username equals i.IDMPM.ToString()
                    where i.Channel == "H2"
                    select p.DeviceToken
                 ).ToList());
            }

            if (article.H3)
            {
                deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join e in _externalUserRepository.GetAll()
                    on p.Username equals e.UserName
                    select p.DeviceToken
                 ).ToList());
            }



            var data = "ARTICLE,"+article.Id+","+article.Title;
            foreach (var deviceToken in deviceTokens)
            {
                using (var fcm = new FcmSender(AppConstants.ServerKey, AppConstants.SenderID))
                {
                    var notification = AppHelpers.CreateNotification(data);
                    await fcm.SendAsync(deviceToken, notification);
                }
            }
        }
    }
}
