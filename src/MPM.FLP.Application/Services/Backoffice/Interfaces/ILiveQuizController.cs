using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface ILiveQuizController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        LiveQuizzes GetByIDBackoffice(Guid guid);
        Task<LiveQuizzes> Create(LiveQuizzes model, IEnumerable<IFormFile> images);
        Task<LiveQuizzes> Edit(LiveQuizzes model, IEnumerable<IFormFile> images);
        String Grid_Destroy(Guid guid);

        BaseResponse GetAllQuestion(Pagination request);
        Task<LiveQuizQuestions> CreateQuestion(QuestionsVM item, IEnumerable<IFormFile> images);
        Task<LiveQuizQuestions> EditQuestion(QuestionsVM item, IEnumerable<IFormFile> images);
        String DestroyQuestion(Guid guid);

        ActionResult DownloadTemplate();
        Task<String> ImportExcel(LiveQuizzes model, IEnumerable<IFormFile> files);
        List<LiveQuizAssignments> InsertUser([FromForm]Guid id, [FromForm]List<int> selectedUser);
        BaseResponse getAssignedUser(Guid id, [FromQuery] Pagination request);
        List<Jabatans> GetJabatan(string channel);
        List<Kotas> GetKota();
        List<Dealers> GetDealer(string Kota);
        List<LiveQuizAssignments> InsertAllUser(Guid id);
        String RemoveUser([FromForm]Guid id, [FromForm]List<int> selectedUser);
    }
}