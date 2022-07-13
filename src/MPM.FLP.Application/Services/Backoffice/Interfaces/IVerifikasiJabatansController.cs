using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IVerifikasiJabatansController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        VerifikasiJabatans GetByIDBackoffice(Guid guid);
        List<Jabatans> Get_IdGroupJabatan();
        Task<VerifikasiJabatans> Create(VerifikasiJabatans model);
        VerifikasiJabatans Edit(VerifikasiJabatans model);
        String Destroy(Guid guid);
        List<HomeworkQuizDto> GetAllHistories(Pagination request);
        HomeworkQuizHistories GetHistoryByID(Guid guid);
        BaseResponse GetAllQuestion(Pagination request);
        VerifikasiJabatanQuestions GetQuestionByID(Guid guid);
        Task<VerifikasiJabatanQuestions> CreateQuestion(QuestionsVM item, IEnumerable<IFormFile> images);
        Task<VerifikasiJabatanQuestions> EditQuestion(QuestionsVM item, IEnumerable<IFormFile> images);
        String DestroyQuestion(Guid guid);
        Task<String> ImportExcel(Guid guid, IEnumerable<IFormFile> files);
    }
}