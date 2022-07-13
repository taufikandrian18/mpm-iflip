using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using MPM.FLP.Authorization.Users;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using MPM.FLP.Services.Dto;
using System.Collections.Generic;
using MPM.FLP.FLPDb;

namespace MPM.FLP.Services.Backoffice
{
    public class InternalUsersController : FLPAppServiceBase, IInternalUsersController
    {
        private readonly InternalUserAppService _appService;
        private readonly UserManager _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly JabatanAppService _jabatanAppService;

        public InternalUsersController(InternalUserAppService appService, UserManager userManager, IHostingEnvironment hostingEnvironment, JabatanAppService jabatanAppService)
        {
            _appService = appService;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _jabatanAppService = jabatanAppService;
        }

        [HttpGet("/api/services/app/backoffice/InternalUsers/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Nama.Contains(request.Query) || x.NoKTP.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            if(!string.IsNullOrEmpty(request.IdJabatan)){
                query = query.Where(x=> x.IDJabatan.ToString() == request.IdJabatan);
            }

            if (!string.IsNullOrEmpty(request.Channel))
            {
                query = query.Where(x => x.Channel.Contains(request.Channel));
            }

            if (!string.IsNullOrEmpty(request.Jabatan))
            {
                query = query.Where(x => x.Jabatan.Contains(request.Jabatan));
            }

            if (!string.IsNullOrEmpty(request.Kota))
            {
                query = query.Where(x => x.DealerKota.Contains(request.Kota));
            }

            if (!string.IsNullOrEmpty(request.KodeDealer))
            {
                query = query.Where(x => x.KodeDealerMPM.Contains(request.KodeDealer));
            }

            if (request.IsActive != null) {
                if(request.IsActive == true){
                    query = query.Where(x=> x.DeletionTime == null);
                } else {
                    query = query.Where(x=> x.DeletionTime != null);
                }
            }

            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/Users")]
        public BaseResponse GetAllUsers([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Nama.Contains(request.Query) || x.NoKTP.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/InternalUsers/getByID")]
        public Task<InternalUserDto> GetByIDBackoffice(int guid)
        {
            return _appService.GetById(guid);
        }

        [HttpGet("/api/services/app/backoffice/InternalUsers/getJabatan")]
        public List<Jabatans> GetJabatan()
        {
            return _jabatanAppService.GetAll().ToList();
        }

        [HttpPut("/api/services/app/backoffice/InternalUsers/update")]
        public async Task<InternalUserDto> Edit([FromForm]InternalUserDto model)
        {
            if (model != null)
            {
                UpdateInternalUserDto item = new UpdateInternalUserDto{
                    AbpUserId = (int)model.AbpUserId,
                    IsActive = model.IsActive,
                    LastModifierUser = _userManager.Users.FirstOrDefault(x => x.UserName == "admin"),
                    LastModificationTime = DateTime.Now
                };

                await _appService.Update(item);
            }
            return model;
        }

        [HttpGet("/api/services/app/backoffice/InternalUsers/exportExcel")]
        public ActionResult ExportExcel()
        {
            string excelName = "InternalUsers.xlsx";

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Internal Users");
                    var user = _userManager.Users.FirstOrDefault(x => x.UserName == "admin");
                    var roles = _userManager.GetRolesAsync(user).Result.ToList();
                    string channel = "";

                    if (roles.FirstOrDefault().Contains("H1"))
                    {
                        channel = "H1";
                    }
                    else if (roles.FirstOrDefault().Contains("H2"))
                    {
                        channel = "H2";
                    }

                    var task = Task.Run(() => _appService.GetAllInternalUsers(channel));

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Id MPM";
                    workSheet.Cells[1, 2].Value = "Id Honda";
                    workSheet.Cells[1, 3].Value = "Nama";
                    workSheet.Cells[1, 4].Value = "Gender";
                    workSheet.Cells[1, 5].Value = "Alamat";
                    workSheet.Cells[1, 6].Value = "Channel";
                    workSheet.Cells[1, 7].Value = "Handphone";
                    workSheet.Cells[1, 8].Value = "Jabatan";
                    workSheet.Cells[1, 9].Value = "Nama Dealer";
                    workSheet.Cells[1, 10].Value = "Kota Dealer";

                    int row = 2;
                    foreach(var result in task.Result)
                    {
                        workSheet.Cells[row, 1].Value = result.IDMPM;
                        workSheet.Cells[row, 2].Value = result.IDHonda;
                        workSheet.Cells[row, 3].Value = result.Nama;
                        workSheet.Cells[row, 4].Value = result.Gender;
                        workSheet.Cells[row, 5].Value = result.Alamat;
                        workSheet.Cells[row, 6].Value = result.Channel;
                        workSheet.Cells[row, 7].Value = result.Handphone;
                        workSheet.Cells[row, 8].Value = result.Jabatan;
                        workSheet.Cells[row, 9].Value = result.DealerName;
                        workSheet.Cells[row, 10].Value = result.DealerKota;

                        row++;
                    }

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