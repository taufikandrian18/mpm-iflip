using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface ISalesDevelopmentContestController : IApplicationService
    {
        List<string> Cascading_Get_Channel();
        List<string> Cascading_Get_Karesidenan(string channel);
        List<string> Cascading_Get_Kota(string channel, string karesidenan);
        List<Dealers> Cascading_Get_Dealer(string channel, string karesidenan, string kota);
        List<InternalUsers> Get_User(string channel, string karesidenan, string kota, string dealer);
        Task<InboxMessages> Create(InboxMessages model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos);
        String InsertRecipient(string id, List<InternalUsersVM> selectedUser);
        List<InternalUsersVM> GetUsers(InboxMessages model);
        List<InboxAttachments> Grid_ReadAttachment(InboxMessages model);
        InboxMessages Edit(InboxMessages model);
        InboxMessages EditAttachment(Guid Id, IEnumerable<IFormFile> files);
        List<InboxMessages> Grid_Read();
        String Grid_Destroy(Guid guid);
        List<InboxAttachments> GridAttachment_Read(Guid modelId);
        String GridAttachment_Destroy(Guid modelId);
    }
}