using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace MPM.FLP.Services.Backoffice
{
    public interface IHomeworkQuizController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        HomeworkQuizzes GetByIDBackoffice(Guid guid);
        Task<HomeworkQuizzes> Create(HomeworkQuizzes model, IEnumerable<IFormFile> images);
        Task<HomeworkQuizzes> Edit(HomeworkQuizzes model, IEnumerable<IFormFile> images);
        String DestroyBackoffice(Guid guid);

        BaseResponse GetAllQuestions(Guid guid, Pagination request);
        HomeworkQuizQuestions GetQuestionByID(Guid guid);
        Task<HomeworkQuizQuestions> CreateQuestion(HomeworkQuizQuestions model, IEnumerable<IFormFile> images);
        Task<HomeworkQuizQuestions> EditQuestion(HomeworkQuizQuestions model, IEnumerable<IFormFile> images);
        String DestroyQuestion(Guid guid);

        ActionResult DownloadTemplate();
        Task<String> ImportExcel(Guid Id, IEnumerable<IFormFile> files);
        //List<InternalUsersVM> GetAssignedUser(Guid id);
        List<Kotas> getKota();
        List<Dealers> getDealer(string Kota);
        BaseResponse getAssignedUser(Guid id, [FromQuery] Pagination request);
        List<HomeworkQuizAssignments> InsertUser(Guid id, List<int> selectedUser);
        List<HomeworkQuizAssignments> InsertAllUser(Guid id);
        string RemoveUser([FromForm]Guid id, [FromForm]List<int> selectedUser);
        List<HomeworkQuizAssignments> RemoveAllUser(Guid id);
        List<HomeworkQuizHistories> Grid_Read_HwQuizScore(Pagination model);
        Task<ActionResult> DownloadScore(Guid id);
    }
}