using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.DataValidation;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Globalization;

namespace MPM.FLP.Services.Backoffice
{
    public class MasterPointsHistoriesController : FLPAppServiceBase, IMasterPointsHistoriesController
    {
        private readonly SalesPeopleDevelopmentContestAppService _appService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly InternalUserAppService _internalUserAppService;

        public MasterPointsHistoriesController(SalesPeopleDevelopmentContestAppService pointsAppService, IHostingEnvironment hostingEnvironment, InternalUserAppService internalUserAppService)
        {
            _appService = pointsAppService;
            _hostingEnvironment = hostingEnvironment;
            _internalUserAppService = internalUserAppService;
        }

        [HttpGet("/api/services/app/backoffice/MasterPointsHistories/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAllPointHistory();

            var count = query.Count();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.CreatorUsername.Contains(request.Query) || x.Point.ToString() == request.Query);
            }

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/MasterPointsHistories/getByID")]
        public SPDCPointHistories GetByIDBackoffice(Guid guid)
        {
            return _appService.GetAllPointHistory().Where(x => x.Id == guid).FirstOrDefault();
        }

        [HttpPost("/api/services/app/backoffice/MasterPointsHistories/create")]
        public async Task<String> Create([FromForm]IEnumerable<IFormFile> files)
        {
            if (files.Count() > 0)
            {
                var file = files.FirstOrDefault();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Template"];
                        var rowCount = worksheet.Dimension.Rows;
                        var categories = _appService.GetAllMasterPoint().Where(x => string.IsNullOrEmpty(x.DeleterUsername));

                        for (int row = 2; row <= rowCount; row++)
                        {
                            int i = 3;
                            foreach (var category in categories)
                            {
                                int idmpm = int.Parse(worksheet.Cells[row, 1].Value.ToString());
                                var period = worksheet.Cells[row, 2].Value.ToString();
                                DateTime dateTime;
                                DateTime periode = new DateTime();
                                if (DateTime.TryParseExact(period, "dd/MM/yyyy", new CultureInfo("id-ID"), DateTimeStyles.None, out dateTime))
                                {
                                    periode = DateTime.ParseExact(period, "dd/MM/yyyy", null);
                                }
                                else
                                {
                                    long dateNum = long.Parse(period);
                                    periode = DateTime.FromOADate(dateNum);
                                }

                                //var masterPoint = worksheet.Cells[row, 3].Value.ToString();
                                var masterPoint = worksheet.Cells[1, i].Value.ToString();
                                var mpId = _appService.GetAllMasterPoint().Where(x => x.Title == masterPoint && string.IsNullOrEmpty(x.DeleterUsername)).Select(x => x.Id).SingleOrDefault();
                                //var point = int.Parse(worksheet.Cells[row, 4].Value.ToString());
                                var point = int.Parse(worksheet.Cells[row, i].Value.ToString());
                                SPDCPointHistories clubCommunities = new SPDCPointHistories
                                {
                                    Id = Guid.NewGuid(),
                                    CreationTime = DateTime.Now,
                                    CreatorUsername = "admin",
                                    LastModifierUsername = "admin",
                                    LastModificationTime = DateTime.Now,
                                    DeleterUsername = "",
                                    IDMPM = idmpm,
                                    SPDCMasterPointId = mpId,
                                    Point = point,
                                    Periode = periode
                                };
                                _appService.CreatePointHisotry(clubCommunities);
                                i++;
                            }
                        }
                    }
                }
            }

            return "Proses Berhasil";
        }

        [HttpPut("/api/services/app/backoffice/MasterPointsHistories/update")]
        public SPDCPointHistories Edit(SPDCPointHistories model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.SPDCMasterPoints = null;

                _appService.UpdatePointHistory(model);
            }

            return model;
        }

        [HttpDelete("/api/services/app/backoffice/MasterPointsHistories/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDeletePointHistory(guid, "admin");
            return "Successfully deleted";
        }
    }
}