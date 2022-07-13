using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using MPM.FLP.Services.Dto;
using MPM.FLP.Authorization.Users;

namespace MPM.FLP.Services.Backoffice
{
    public class ExternalUsersController : FLPAppServiceBase, IExternalUsersController
    {
        private readonly  ExternalUserAppService _appService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager _userManager;

        public ExternalUsersController(ExternalUserAppService appService, IHostingEnvironment hostingEnvironment, UserManager userManager)
        {
            _appService = appService;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        [HttpGet("/api/services/app/backoffice/ExternalUsers/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAllBackoffice();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query) || x.UserName.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ExternalUsers/getByID")]
        public Task<ExternalUserDto> GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPut("/api/services/app/backoffice/ExternalUsers/update")]
        public async Task<ExternalUserDto> EditExternal([FromForm]ExternalUserDto model)
        {
            if (model != null)
            {
                UpdateExternalUserDto item = new UpdateExternalUserDto();
                item.Id = Task.Run(() => _appService.GetAll()).Result.SingleOrDefault(x => x.AbpUserId == model.AbpUserId).Id;
                item.AbpUserId = (int)model.AbpUserId;
                item.IsKTPVerified = model.IsKTPVerified;
                item.IsActive = model.IsActive;
                item.LastModifierUser = _userManager.Users.FirstOrDefault(x => x.UserName == "admin");
                item.LastModificationTime = DateTime.UtcNow.AddHours(7);
                await _appService.Update(item);

                // _appService.Update(model);
            }
            return model;
        }

        [HttpGet("/api/services/app/backoffice/ExternalUsers/exportExcel")]
        public ActionResult ExportExcel()
        {
            //string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "ExternalUsers.xlsx";

            var stream = new MemoryStream();
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("ExternalUsers");
                    

                    var task = Task.Run(() => _appService.GetAll());

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Nama";
                    workSheet.Cells[1, 2].Value = "Nama Toko";
                    workSheet.Cells[1, 3].Value = "Alamat";
                    workSheet.Cells[1, 4].Value = "Longitude";
                    workSheet.Cells[1, 5].Value = "Latitude";
                    workSheet.Cells[1, 6].Value = "Channel";
                    workSheet.Cells[1, 7].Value = "Handphone";
                    workSheet.Cells[1, 8].Value = "Jabatan";
                    workSheet.Cells[1, 9].Value = "Email";

                    int row = 2;
                    foreach (var result in task.Result)
                    {
                        workSheet.Cells[row, 1].Value = result.Name;
                        workSheet.Cells[row, 2].Value = result.ShopName;
                        workSheet.Cells[row, 3].Value = result.Address;
                        workSheet.Cells[row, 4].Value = result.Longitude;
                        workSheet.Cells[row, 5].Value = result.Latitude;
                        workSheet.Cells[row, 6].Value = result.Channel;
                        workSheet.Cells[row, 7].Value = result.Handphone;
                        workSheet.Cells[row, 8].Value = result.Jabatan;
                        workSheet.Cells[row, 9].Value = result.Email;

                        row++;
                    }

                    // workSheet.Column(1).AutoFit();
                    // workSheet.Column(2).AutoFit();
                    // workSheet.Column(3).AutoFit();
                    // workSheet.Column(4).AutoFit();
                    // workSheet.Column(5).AutoFit();
                    // workSheet.Column(6).AutoFit();
                    // workSheet.Column(7).AutoFit();
                    // workSheet.Column(8).AutoFit();
                    // workSheet.Column(9).AutoFit();
                    package.Save();
                }

            stream.Position = 0;
            
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") 
            { 
                FileDownloadName = excelName
            };        
        }    
    }
}