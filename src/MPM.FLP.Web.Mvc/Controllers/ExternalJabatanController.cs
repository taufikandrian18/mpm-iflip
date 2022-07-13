using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Controllers;
using MPM.FLP.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class ExternalJabatanController : FLPControllerBase
    {
        private readonly ExternalJabatanAppService _appService;

        public ExternalJabatanController(ExternalJabatanAppService appService)
        {
            _appService = appService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            ExternalJabatans model = new ExternalJabatans();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExternalJabatans model, string submit)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";
                
                _appService.Create(model);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetById(id);

            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(ExternalJabatans model, string submit)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return RedirectToAction("Index");
        }


        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x=>x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, Articles item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id,this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

    }
}