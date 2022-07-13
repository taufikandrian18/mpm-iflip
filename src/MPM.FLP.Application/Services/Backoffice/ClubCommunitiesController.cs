using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml.DataValidation;

namespace MPM.FLP.Services.Backoffice
{
    public class ClubCommunitiesController : FLPAppServiceBase, IClubCommunitiesController
    {
        private readonly ClubCommunityAppService _appService;
        private readonly ClubCommunityCategoryAppService _categoriesAppService;

        private readonly IHostingEnvironment _hostingEnvironment;

        public ClubCommunitiesController(ClubCommunityAppService appService, ClubCommunityCategoryAppService categoriesAppService, IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _categoriesAppService = categoriesAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("/api/services/app/backoffice/ClubCommunities/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAll();

            if(!string.IsNullOrEmpty(request.Query)){
               query = query.Where(x=> x.ContactNumber.Contains(request.Query) || x.ContactPerson.Contains(request.Query) || x.CreatorUsername.Contains(request.Query) || x.Email.Contains(request.Query) || x.Name.Contains(request.Query));
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ClubCommunities/getByID")]
        public ClubCommunities GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/ClubCommunities/create")]
        public ClubCommunities Create([FromForm]ClubCommunitiesVM data)
        {
            ClubCommunities model = new ClubCommunities();
            model.Id = Guid.NewGuid();
            model.CreationTime = DateTime.Now;
            model.CreatorUsername = "admin";
            model.LastModifierUsername = "admin";
            model.LastModificationTime = DateTime.Now;
            model.DeleterUsername = "";
            model.ClubCommunityCategoryId = data.ClubCommunityCategoryId;
            model.Name = data.Name;
            model.ContactPerson = data.ContactPerson;
            model.ContactNumber = data.ContactNumber;
            model.Email = data.Email;
            model.Kota = data.Kota;

            _appService.Create(model);

            return model;
        }

        [HttpPost("/api/services/app/backoffice/ClubCommunities/import")]
        public async Task<string> Import(IEnumerable<IFormFile> files)
        {
            List<ClubCommunities> clubCommunitiesArr = new List<ClubCommunities>();
            if (files.Count() > 0)
            {
                var file = files.FirstOrDefault();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Template Club Communities"];

                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var clubName = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var categoriesClub = worksheet.Cells[row, 2].Value?.ToString().Trim();
                            var contactPerson = worksheet.Cells[row, 3].Value?.ToString().Trim();
                            var contactNumber = worksheet.Cells[row, 4].Value?.ToString().Trim();
                            var email = worksheet.Cells[row, 5].Value?.ToString().Trim();
                            var city = worksheet.Cells[row, 6].Value?.ToString().Trim();

                            if (clubName == null)
                            {
                                return "Data nama klub pada baris " + row + " masih kosong";
                            }
                            if (categoriesClub == null)
                            {
                                return "Data nama kategori klub pada baris " + row + " masih kosong";
                            }
                            if (contactPerson == null)
                            {
                                return "Data nama kontak pada baris " + row + " masih kosong";
                            }
                            if (contactNumber == null)
                            {
                                return "Data nomor kontak pada baris " + row + " masih kosong";
                            }
                            if (email == null)
                            {
                                return "Data email pada baris " + row + " masih kosong";
                            }
                            if (city == null)
                            {
                                return "Data kota pada baris " + row + " masih kosong";
                            }

                            var categoriesClubId = Guid.NewGuid();
                            try
                            {
                                categoriesClubId = _categoriesAppService.GetAll().FirstOrDefault(x => x.Name == categoriesClub && string.IsNullOrEmpty(x.DeleterUsername)).Id;
                            }
                            catch (Exception ex)
                            {
                                var c = ex;
                            }
                            ClubCommunities clubCommunities = new ClubCommunities
                            {
                                Id = Guid.NewGuid(),
                                Name = clubName,
                                ClubCommunityCategoryId = categoriesClubId,
                                ContactPerson = contactPerson,
                                ContactNumber = contactNumber,
                                Email = email,
                                Kota = city,
                                CreationTime = DateTime.Now,
                                CreatorUsername = "admin",
                                LastModifierUsername = "admin",
                                LastModificationTime = DateTime.Now,
                                DeleterUsername = ""
                            };
                            _appService.Create(clubCommunities);
                            clubCommunitiesArr.Add(clubCommunities);
                        }
                        return "Success Import";
                    }
                }
            }
            return "Files empty";
        }

        [HttpPost("/api/services/app/backoffice/ClubCommunities/update")]
        public ClubCommunities EditBackoffice(ClubCommunities model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/ClubCommunities/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/ClubCommunities/downloadTemplate")]
        public ActionResult DownloadTemplate()
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Template Club Communities.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Template Club Communities");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Nama Klub";
                    workSheet.Cells[1, 2].Value = "Kategori Klub";
                    workSheet.Cells[1, 3].Value = "Nama Kontak";
                    workSheet.Cells[1, 4].Value = "Nomor Kontak";
                    workSheet.Cells[1, 5].Value = "E-mail";
                    workSheet.Cells[1, 6].Value = "Kota";

                    //workSheet.Cells[2, 1].Value = "Honda Club";
                    //workSheet.Cells[2, 2].Value = "fed5b102-ddbf-46bd-9936-18f07a4f697d";
                    //workSheet.Cells[2, 3].Value = "MPM";
                    //workSheet.Cells[2, 4].Value = "088812345678";
                    //workSheet.Cells[2, 5].Value = "flpmpm@email.com";
                    //workSheet.Cells[2, 6].Value = "Surabaya";

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();
                    workSheet.Column(6).AutoFit();

                    var validation = workSheet.DataValidations.AddListValidation("B2:B200");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    validation.ErrorTitle = "An invalid value was entered";
                    validation.Error = "Select a value from the list";

                    var workSheetCategories = package.Workbook.Worksheets.Add("Kategori Klub");
                    workSheetCategories.Cells[1, 1].Value = "Kategori Klub";
                    workSheetCategories.Cells[1, 2].Value = "Kategori Klub Id";

                    var categories = _categoriesAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
                    int i = 2;
                    foreach (var category in categories)
                    {
                        workSheetCategories.Cells[i, 1].Value = category.Name;
                        workSheetCategories.Cells[i, 2].Value = category.Id;
                        validation.Formula.Values.Add(category.Name);
                        i++;
                    }

                    workSheetCategories.Column(1).AutoFit();
                    workSheetCategories.Column(2).AutoFit();
                    package.Save();
                }
            }
            catch (Exception ex)
            {

            }

            stream.Position = 0;

            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = excelName
            };
        }
    }
}