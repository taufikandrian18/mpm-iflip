using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class BASTReportAppService : FLPAppServiceBase, IBASTReportAppService
    {
        private readonly IRepository<BASTReport, Guid> _BASTReportRepository;

        public BASTReportAppService(IRepository<BASTReport, Guid> BASTReportRepository)
        {
            _BASTReportRepository = BASTReportRepository;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _BASTReportRepository.GetAll().Where(x => x.DeletionTime == null);
            
            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }
        public BASTReport GetById(Guid id)
        {
            var report = _BASTReportRepository.FirstOrDefault(x => x.Id == id);
            return report;
        }

        public void Create(BASTReportCreateDto input)
        {
            var report = ObjectMapper.Map<BASTReport>(input);
            report.CreationTime = DateTime.Now;
            report.CreatorUsername = this.AbpSession.UserId.ToString();
            _BASTReportRepository.Insert(report);
        }

        public void Update(BASTReportUpdateDto input)
        {
            var report = _BASTReportRepository.Get(input.Id);
            report.GUIDBAST = input.GUIDBAST;
            report.GUIDReporter = input.GUIDReporter;
            report.NamaReporter = input.NamaReporter;
            report.GUIDPenerima = input.GUIDPenerima;
            report.NamaPenerima = input.NamaPenerima;
            report.GUIDUpdater = input.GUIDUpdater;
            report.NamaUpdater = input.NamaUpdater;
            report.KodeAHM = input.KodeAHM;
            report.KodeMPM = input.KodeMPM;
            report.Feedback = input.Feedback;
            report.JumlahDatang = input.JumlahDatang;
            report.JumlahReject = input.JumlahReject;
            report.LastModifierUsername = this.AbpSession.UserId.ToString();
            report.LastModificationTime = DateTime.Now;

            _BASTReportRepository.Update(report);
        }

        public void SoftDelete(Guid id)
        {
            var report = _BASTReportRepository.FirstOrDefault(x => x.Id == id);
            report.DeleterUsername = this.AbpSession.UserId.ToString();
            report.DeletionTime = DateTime.Now;
            _BASTReportRepository.Update(report);
        }
    }
}
