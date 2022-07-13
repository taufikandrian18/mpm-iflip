using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface ISelfRecordingController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        SelfRecordings GetByIDBackoffice(Guid guid);
        Task<SelfRecordings> Create(SelfRecordings model, IEnumerable<IFormFile> images);
        SelfRecordings Edit(SelfRecordings model);
        String Destroy(Guid guid);

        AssignmentDealerVM Result(Guid id);
        List<RoleplayResultVM> Grid_Result_Read(AssignmentDealerVM item);
        List<RoleplayResultVM> Grid_Result_Detail_Read(string idmpm, Guid idRoleplay);

        BaseResponse Detail(Pagination request);
        Task<SelfRecordingDetails> CreateDetail(SelfRecordingDetails model);
        SelfRecordingDetails EditDetail(SelfRecordingDetails model);
        String DestroyDetail(Guid guid);
        List<Kotas> Cascading_Get_Kota();
        ActionResult DownloadTemplate(SelfRecordings model);
        Task<String> ImportExcel(SelfRecordings model, string submit, IEnumerable<IFormFile> files);

        List<SelfRecordingAssignments> Get_Assigned_Dealer(string id, string channel);
        List<SelfRecordingAssignments> InsertDealer(Guid id, List<string> selectedDealer);
        List<SelfRecordingAssignments> InsertAllDealer(Guid id);
        String RemoveDealer(Guid id, List<DealerVM> selectedDealer);
        String RemoveAllDealer(Guid id);

        Double CalculatePoint([FromBody] List<ValidationVM> validations, Guid id);
        Task<String> GetMandatory(Guid id);
        List<RoleplayResultDetailVM> Grid_Validate_Read(SelfRecordingResults item);
        ActionResult DownloadTemplateAssignee();
        Task<String> ImportExcelAssignee(Guid parentId, IEnumerable<IFormFile> files);
    }
}