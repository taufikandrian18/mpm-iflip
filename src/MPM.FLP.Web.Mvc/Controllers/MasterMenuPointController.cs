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
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Web;
using MPM.FLP.Services.Dto;
using MPM.FLP.Web.Models.FLPMPM;
using Abp.Web.Models;
using Newtonsoft.Json;

namespace MPM.FLP.Web.Mvc.Controllers
{
    public class MasterMenuPointController : FLPControllerBase
    {
        private readonly PointAppService _appService;

        public MasterMenuPointController(PointAppService appService)
        {
            _appService = appService;
        }

        #region Main

        public IActionResult Index()
        {
            return View();
        }

        [DontWrapResult(WrapOnError = false, WrapOnSuccess = false)]
        public IActionResult GetContentType([DataSourceRequest]DataSourceRequest request)
        {
            List<ItemDropDown> itemDropDowns = new List<ItemDropDown>();

            ItemDropDown itemDropDown = new ItemDropDown("Article", "article");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Guide", "guide");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Sales Program", "salesprogram");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Service Program", "serviceprogram");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Sales Talk", "salestalk");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Service Talk Flyer", "servicetalkflyer");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Brand Campaign", "brandcampaign");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Info Main Dealer", "infomaindealer");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Homework Quiz", "homeworkquiz");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Live Quiz", "livequiz");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Role Play", "roleplay");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Self Recording", "selfrecording");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Online Magazine", "onlinemagazine");
            itemDropDowns.Add(itemDropDown);

            string serializer = JsonConvert.SerializeObject(itemDropDowns.ToDataSourceResult(request));

            return Content(serializer, "application/json");
        }

        [DontWrapResult(WrapOnError = false, WrapOnSuccess = false)]
        public IActionResult GetActivityType([DataSourceRequest]DataSourceRequest request)
        {
            List<ItemDropDown> itemDropDowns = new List<ItemDropDown>();

            ItemDropDown itemDropDown = new ItemDropDown("Read", "read");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Write", "write");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Take", "take");
            itemDropDowns.Add(itemDropDown);

            string serializer = JsonConvert.SerializeObject(itemDropDowns.ToDataSourceResult(request));

            return Content(serializer, "application/json");
        }

        public IActionResult Create()
        {
            AddPointConfigurationDto model = new AddPointConfigurationDto();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDefault(AddPointConfigurationDto model, string submit)
        {
            if (model != null)
            {
                var item = (await _appService.GetActivePointConfigurations()).Where(x => x.ContentType == model.ContentType && x.IsDefault == true);
                if (item.Count() > 0)
                {
                    return Json(new { success = false });
                }
                model.IsDefault = true;
                await _appService.AddPointConfiguration(model);
            }
            return Json(new { success = true });
        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetPointConfigurationById(id);

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePointConfigurationDto model, string submit)
        {
            if (model != null)
            {
                await _appService.UpdatePointConfiguration(model);
            }
            return Json(new { success = true });
        }

        public async Task<IActionResult> Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            var items = (await _appService.GetAll()).Where(x => x.IsDefault == true);
            List<PointConfigurationVM> model = new List<PointConfigurationVM>();

            foreach(var item in items)
            {
                int? ap = null;
                DateTime? efrom = null;
                DateTime? eto = null;

                var tmp = (await _appService.GetAll()).Where(x => x.IsDefault == false && x.ContentType == item.ContentType).OrderBy(x => x.EffDateTo).FirstOrDefault();
                if(tmp != null)
                {
                    ap = tmp.Point;
                    efrom = tmp.EffDateFrom;
                    eto = tmp.EffDateTo;
                }

                model.Add(new PointConfigurationVM
                {
                    Id = item.Id,
                    ContentType = item.ContentType,
                    ActivityType = item.ActivityType,
                    DefaultPoint = item.Point,
                    DefaultThreshold = item.DefaultThreshold,
                    IsDefault = item.IsDefault,
                    ActivePoint = ap,
                    EffDateFrom = efrom,
                    EffDateTo = eto
                });
            }
            DataSourceResult result = model.ToDataSourceResult(request);

            return Json(result);
        }

        public async Task<IActionResult> Grid_Destroy([DataSourceRequest]DataSourceRequest request, PointConfigurationVM item)
        {
            if (ModelState.IsValid)
            {
                await _appService.DeletePointConfiguration(item.Id);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Detail Point

        public async Task<IActionResult> DetailPoint(Guid id)
        {
            var items = (await _appService.GetAll()).Where(x => x.Id == id && x.IsDefault == true).FirstOrDefault();
            return View("DetailPoint",items);
        }
        public async Task<IActionResult> BackToDetail(string content)
        {
            var model = (await _appService.GetAll()).Where(x => x.ContentType == content && x.IsDefault == true).FirstOrDefault();
            //return RedirectToAction("DetailPoint",model);
            return await DetailPoint(model.Id);
        }

        public async Task<IActionResult> Grid_Detail_Read([DataSourceRequest]DataSourceRequest request, string content)
        {
            var items = (await _appService.GetAll()).Where(x => x.ContentType == content);
            
            DataSourceResult result = items.ToDataSourceResult(request);

            return Json(result);
        }

        public async Task<IActionResult> Grid_Detail_Destroy([DataSourceRequest]DataSourceRequest request, PointConfigurationDto item)
        {
            if (ModelState.IsValid)
            {
                await _appService.DeletePointConfiguration(item.Id);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        public async Task<IActionResult> CreatePoint(Guid id)
        {
            id = Guid.Parse(id.ToString().ToUpper());
            var items = (await _appService.GetAll()).Where(x => x.Id == id).SingleOrDefault();
            AddPointConfigurationDto item = new AddPointConfigurationDto();
            item.ContentType = items.ContentType;
            item.ActivityType = items.ActivityType;
            //item.Point = 0;
            //item.DefaultThreshold = 0;
            
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDetailPoint(AddPointConfigurationDto model, string submit)
        {
            if (model != null)
            {
                model.IsDefault = false;
                await _appService.AddPointConfiguration(model);
            }
            var item = (await _appService.GetAll()).Where(x => x.ContentType == model.ContentType && x.IsDefault == true).SingleOrDefault();
            return Json(new { success = true, id = item.Id });
        }

        public async Task<IActionResult> EditPoint(Guid id)
        {
            var item = (await _appService.GetAll()).Where(x => x.Id == id).SingleOrDefault();
            
            if(item.IsDefault == true)
            {
                return View("EditDefaultPoint", item);
            }
            else
            {
                return View(item);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditDetailPoint(PointConfigurationDto model, string submit)
        {
            if (model != null)
            {
                UpdatePointConfigurationDto item = new UpdatePointConfigurationDto
                {
                    Id = model.Id,
                    Point = model.Point,
                    DefaultThreshold = model.DefaultThreshold,
                    EffDateFrom = model.EffDateFrom,
                    EffDateTo = model.EffDateTo,
                    IsDefault = model.IsDefault
                };
                await _appService.UpdatePointConfiguration(item);
            }
            var id = (await _appService.GetAll()).Where(x => x.ContentType == model.ContentType && x.IsDefault == true).SingleOrDefault().Id;
            return Json(new { success = true, id = id });
        }
        #endregion
        //#region Claimer

        //public IActionResult ClaimedPeople(Guid id)
        //{
        //    var model = _appService.GetById(id);
        //    return View(model);
        //}
        //public IActionResult Grid_Claimer_Read([DataSourceRequest]DataSourceRequest request, ClaimPrograms model)
        //{

        //    DataSourceResult result = _claimerAppService.GetClaimers(model.Id).ToDataSourceResult(request);

        //    return Json(result);
        //}

        //public IActionResult Grid_Claimer_Destroy([DataSourceRequest]DataSourceRequest request, ClaimerDto item)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //_claimerAppService.SoftDelete(item.Id);
        //    }

        //    return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        //}
        //public IActionResult ApproveClaim(Guid id)
        //{
        //    _claimerAppService.Approve(new ApprovalClaimDto { ClaimProgramClaimerId = id, IsApproved = true });

        //    return Json(new { success = true });
        //}

        //public IActionResult RejectClaim(Guid id)
        //{
        //    _claimerAppService.Approve(new ApprovalClaimDto { ClaimProgramClaimerId = id, IsApproved = false });

        //    return Json(new { success = true });
        //}
        //#endregion



    }
}