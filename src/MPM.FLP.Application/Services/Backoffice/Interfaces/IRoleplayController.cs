using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface IRoleplayController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        RolePlays GetByIDBackoffice(Guid guid);
        Task<RolePlays> Create(RolePlays model, IEnumerable<IFormFile> images);
        RolePlays Edit(RolePlays model);
        String Destroy(Guid guid);
        BaseResponse GetAllDetails(Pagination request);
        RolePlayDetails GetDetailByID(Guid guid);
        Task<RolePlayDetails> CreateDetail(RolePlayDetails model);
        RolePlayDetails EditDetail(RolePlayDetails model);
        String DestroyDetail(Guid guid);

        List<DealerVM> GetAssignedDealer(Guid id);
        List<Kotas> getKota();
        List<RolePlayAssignments> InsertDealer(Guid id, List<string> selectedDealer);
        List<RolePlayAssignments> InsertAllDealer(Guid id);
        String RemoveDealer(Guid id, List<String> selectedDealer);
        String RemoveAllDealer(Guid id);

        AssignmentDealerVM Result(Guid id);
        List<RoleplayResultVM> Grid_Result_Read(AssignmentDealerVM roleplay);
        List<RoleplayResultVM> Grid_Result_Detail_Read(string idmpm, Guid idRoleplay);

        Task<Double> CalculatePoint(List<ValidationVM> validations, Guid id);
        Task<String> GetMandatory(Guid id);
        List<RoleplayResultDetailVM> Grid_Validate_Read(RolePlayResults rolePlayResults);
        ActionResult DownloadTemplateAssignee();
        Task<String> ImportExcelAssignee(Guid parentId, IEnumerable<IFormFile> files);
    }
}
