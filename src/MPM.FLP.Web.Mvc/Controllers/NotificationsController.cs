using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using MPM.FLP.Controllers;
using MPM.FLP.Services;
using System.Collections;
using System.Collections.Generic;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MPM.FLP.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using MPM.FLP.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using MPM.FLP.Authorization.Users;
using Abp.Runtime.Security;
using MPM.FLP.Authorization.Roles;
using Abp.Runtime.Session;
using System.Web;
using Abp.Extensions;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class NotificationsController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly WebNotificationAppService _appService;
        private readonly ArticleAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public NotificationsController(WebNotificationAppService appService, UserManager users)
        {
            _userManager = users;
            _appService = appService;
        }

        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            return View();
        }

        public JsonResult GetNotifications()
        {
            List<WebNotifications> notifications = new List<WebNotifications>();
            string paramUserName = "";
            var userName = _userManager.Users.FirstOrDefault(y => y.Id == this.User.Identity.GetUserId());
            if (userName == null)
            {
                paramUserName = "admin";
            }
            else {
                paramUserName = userName.UserName;
            }
            notifications = _appService.GetAll().Where(x=> x.ReceiverUsername == paramUserName).OrderBy(x=>x.IsRead).ToList();
            return Json(notifications);
        }

        public JsonResult SetRead(Guid notiId)
        {
            _appService.SetRead(notiId);
            return Json(new { success = true });
        }
        //[HttpPost]
        //public async Task<IActionResult> Create(Articles model, string submit, IEnumerable<IFormFile> images)
        //{
        //    if (model != null)
        //    {
        //        if (model.Title == null)
        //        {
        //            TempData["alert"] = "Judul masih kosong";
        //            TempData["success"] = "";
        //            return RedirectToAction("Create", model);
        //        }

        //        var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
        //        var roles = _userManager.GetRolesAsync(user).Result.ToList();

        //        string resource = null;
        //        if (roles.FirstOrDefault().Contains("H1"))
        //        {
        //            resource = "H1";
        //        }
        //        else if (roles.FirstOrDefault().Contains("H2"))
        //        {
        //            resource = "H2";
        //        }
        //        else if (roles.FirstOrDefault().Contains("H3"))
        //        {
        //            resource = "H3";
        //        }
        //        else if (roles.FirstOrDefault().Contains("HC3"))
        //        {
        //            resource = "HC3";
        //        }

        //        model.Id = Guid.NewGuid();
        //        model.CreationTime = DateTime.Now;
        //        model.CreatorUsername = this.User.Identity.Name;
        //        model.LastModifierUsername = this.User.Identity.Name;
        //        model.LastModificationTime = DateTime.Now;
        //        model.DeleterUsername = "";
        //        model.ViewCount = 0;
        //        model.Resource = resource;


        //        if (images.Count() > 0)
        //        {
        //            foreach (var image in images)
        //            {
        //                model.ArticleAttachments.Add(await InsertToAzure(image, model, "Create"));
        //            }

        //            model.FeaturedImageUrl = model.ArticleAttachments.First().StorageUrl;
        //        }

        //        //_appService.Create(model);
        //    }
        //    return RedirectToAction("Index");
        //}

    }
}