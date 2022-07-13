using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using MPM.FLP.Services.Dto;
using MPM.FLP.FLPDb;

namespace MPM.FLP.Services.Backoffice
{
    public class MasterMenuPointController : FLPAppServiceBase, IMasterMenuPointController
    {
        private readonly PointAppService _appService;

        public MasterMenuPointController(PointAppService appService)
        {
            _appService = appService;
        }

        #region Main
        [HttpGet("/api/services/app/backoffice/MasterMenuPoint/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAllBackoffice();

            var count = query.Count();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.CreatorUsername.Contains(request.Query) || x.ContentType.Contains(request.Query) || x.ActivityType.Contains(request.Query));
            }

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/MasterMenuPoint/getByID")]
        public PointConfigurations GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpGet("/api/services/app/backoffice/MasterMenuPoint/getContentTypes")]
        public List<ItemDropDown> GetContentType()
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

            return itemDropDowns;
        }

        [HttpGet("/api/services/app/backoffice/MasterMenuPoint/getActivityTypes")]
        public List<ItemDropDown> GetActivityType()
        {
            List<ItemDropDown> itemDropDowns = new List<ItemDropDown>();

            ItemDropDown itemDropDown = new ItemDropDown("Read", "read");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Write", "write");
            itemDropDowns.Add(itemDropDown);
            itemDropDown = new ItemDropDown("Take", "take");
            itemDropDowns.Add(itemDropDown);

            return itemDropDowns;
        }

        [HttpPost("/api/services/app/backoffice/MasterMenuPoint/create")]
        public async Task<AddPointConfigurationDto> CreateDefault([FromForm]AddPointConfigurationDto model)
        {
            var item = (await _appService.GetActivePointConfigurations()).Where(x => x.ContentType == model.ContentType && x.IsDefault == true);
            model.IsDefault = true;
            model = await _appService.AddPointConfiguration(model);
            return model;
        }

        [HttpPut("/api/services/app/backoffice/MasterMenuPoint/update")]
        public async Task<UpdatePointConfigurationDto> Edit(UpdatePointConfigurationDto model)
        {
            if (model != null)
            {
                model = await _appService.UpdatePointConfiguration(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/MasterMenuPoint/destroy")]
        public async Task<String> DestroyBackoffice(Guid guid)
        {
            await _appService.DeletePointConfiguration(guid);
            return "Successfully deleted";
        }
        #endregion

        [HttpGet("/api/services/app/backoffice/MasterMenuPoint/getAllDetails")]
        public List<PointConfigurations> Grid_Detail_Read(string content)
        {
            return _appService.GetAllBackoffice().Where(x => x.ContentType == content).ToList();
        }

        [HttpPost("/api/services/app/backoffice/MasterMenuPoint/createPoint")]
        public async Task<PointConfigurationDto> CreateDetailPoint(AddPointConfigurationDto model)
        {
            if (model != null)
            {
                model.IsDefault = false;
                await _appService.AddPointConfiguration(model);
            }
            return (await _appService.GetAll()).Where(x => x.ContentType == model.ContentType && x.IsDefault == true).SingleOrDefault();
        }

        [HttpPut("/api/services/app/backoffice/MasterMenuPoint/editPoint")]
        public async Task<PointConfigurationDto> EditDetailPoint(PointConfigurationDto model)
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
            return (await _appService.GetAll()).Where(x => x.Id == model.Id).SingleOrDefault();
        }
    }
}