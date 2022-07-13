using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public interface ICSChampionClubsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        CSChampionClubs GetByIDBackoffice(Guid guid);
        Task<CSChampionClubs> Create(CSChampionClubsVM data, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos);
        CSChampionClubs EditBackoffice(CSChampionClubsVM data);
        CSChampionClubs UploadAttachment(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> documents, IEnumerable<IFormFile> videos);
        String DestroyBackoffice(Guid guid);
        BaseResponse GetAllParticipants(Pagination request);
        List<CSChampionClubParticipantsVM> SearchParticipants(int year);
        CSChampionClubRegistrations UpdateRegistration(CSChampionClubRegistrations model);
        List<CSChampionClubAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType);
        String DestroyAttachmentBackoffice(Guid guid);
    }
}