using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using System;
using MPM.FLP.Authorization.Users;
using Microsoft.AspNetCore.Http;

namespace MPM.FLP.Services.Backoffice
{
    public interface IInboxController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        InboxMessages GetByIDBackoffice(Guid guid);
        String InsertRecipient(string id, List<User> selectedUser);
        InboxMessages Edit(InboxMessages model);
        InboxMessages EditAttachment(Guid Id, IEnumerable<IFormFile> files);
        String DestroyBackoffice(Guid guid);
    }
}