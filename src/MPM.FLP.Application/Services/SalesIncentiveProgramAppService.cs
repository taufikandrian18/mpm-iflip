using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class SalesIncentiveProgramAppService : FLPAppServiceBase, ISalesIncentiveProgramAppService
    {
        private readonly IRepository<SalesIncentivePrograms, Guid> _salesIncentiveProgramRepository;
        private readonly IRepository<SalesIncentiveProgramKotas, Guid> _salesIncentiveProgramKotaRepository;
        private readonly IRepository<SalesIncentiveProgramJabatans, Guid> _salesIncentiveProgramJabatanRepository;
        private readonly IRepository<SalesIncentiveProgramAttachments, Guid> _salesIncentiveProgramAttachmentRepository;
        private readonly IRepository<SalesIncentiveProgramAssignee, Guid> _salesIncentiveProgramAssigneeRepository;
        private readonly IRepository<SalesIncentiveProgramTarget, Guid> _salesIncentiveProgramTargetRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public SalesIncentiveProgramAppService(
            IRepository<SalesIncentivePrograms, Guid> salesIncentiveProgramRepository,
            IRepository<SalesIncentiveProgramKotas, Guid> salesIncentiveProgramKotaRepository,
            IRepository<SalesIncentiveProgramJabatans, Guid> salesIncentiveProgramJabatanRepository,
            IRepository<SalesIncentiveProgramAttachments, Guid> salesIncentiveProgramAttachmentRepository,
            IRepository<SalesIncentiveProgramAssignee, Guid> salesIncentiveProgramAssigneeRepository,
            IRepository<SalesIncentiveProgramTarget, Guid> salesIncentiveProgramTargetRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _salesIncentiveProgramRepository = salesIncentiveProgramRepository;
            _salesIncentiveProgramKotaRepository = salesIncentiveProgramKotaRepository;
            _salesIncentiveProgramJabatanRepository = salesIncentiveProgramJabatanRepository;
            _salesIncentiveProgramAttachmentRepository = salesIncentiveProgramAttachmentRepository;
            _salesIncentiveProgramAssigneeRepository = salesIncentiveProgramAssigneeRepository;
            _salesIncentiveProgramTargetRepository = salesIncentiveProgramTargetRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public void Create(SalesIncentivePrograms input)
        {
            var salesId = _salesIncentiveProgramRepository.InsertAndGetId(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Sales Incentive Program", salesId, input.Title, LogAction.Create.ToString(), null, input);
        }

        //public void CreateIncentive(SalesIncentiveProgramsCreateDto input)
        //{
        //        #region Create sales incentive program
        //        var incentive = ObjectMapper.Map<SalesIncentivePrograms>(input);
        //        incentive.CreationTime = DateTime.Now;
        //        incentive.CreatorUsername = this.AbpSession.UserId.ToString();

        //        Guid incentiveId = _salesIncentiveProgramRepository.InsertAndGetId(incentive);
        //    #endregion

        //    #region Create attachment
        //    foreach (var attachment in input.attachments)
        //    {
        //        var _attachment = ObjectMapper.Map<SalesIncentiveProgramAttachments>(attachment);
        //        _attachment.SalesIncentiveProgramId = incentiveId;
        //        _attachment.CreatorUsername = this.AbpSession.UserId.ToString();
        //        _attachment.CreationTime = DateTime.Now;
        //        _salesIncentiveProgramAttachmentRepository.Insert(_attachment);
        //    }
        //    #endregion

        //    #region Create jabatan
        //    foreach (var jabatan in input.jabatans)
        //    {
        //        var _jabatan = ObjectMapper.Map<SalesIncentiveProgramJabatans>(jabatan);
        //        _jabatan.SalesIncentiveProgramId = incentiveId;
        //        _salesIncentiveProgramJabatanRepository.Insert(_jabatan);
        //    }
        //    #endregion

        //    #region Create kota
        //    foreach (var kota in input.kotas)
        //    {
        //        var _kota = ObjectMapper.Map<SalesIncentiveProgramKotas>(kota);
        //        _kota.SalesIncentiveProgramId = incentiveId;
        //        _salesIncentiveProgramKotaRepository.Insert(_kota);
        //    }
        //    #endregion

        //    #region Create assignee
        //    foreach (var assigne in input.assignee)
        //    {
        //        var _assignee = ObjectMapper.Map<SalesIncentiveProgramAssignee>(assigne);
        //        _assignee.SalesIncentiveProgramId = incentiveId;
        //        _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
        //        _assignee.CreationTime = DateTime.Now;
        //        _salesIncentiveProgramAssigneeRepository.Insert(_assignee);
        //    }
        //    #endregion

        //}

        public IQueryable<SalesIncentivePrograms> GetAll()
        {
            return _salesIncentiveProgramRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                .Include(x => x.SalesIncentiveProgramAttachments)
                .Include(x => x.SalesIncentiveProgramKotas)
                .Include(x => x.SalesIncentiveProgramJabatans);
        }

        public List<Guid> AllIdByKotaJabatan(SalesIncentiveProgramGetIdDto input)
        {
            List<Guid> kota = _salesIncentiveProgramKotaRepository.GetAll().Where(x => x.NamaKota.Contains(input.Kota))
                                .Select(x => x.SalesIncentiveProgramId).ToList();

            List<Guid> jabatan = _salesIncentiveProgramJabatanRepository.GetAll().Where(x => x.NamaJabatan == input.Jabatan)
                                .Select(x => x.SalesIncentiveProgramId).ToList();

            return _salesIncentiveProgramRepository.GetAll().Where(x => kota.Contains(x.Id)
                                                       && jabatan.Contains(x.Id)
                                                       && DateTime.Now.Date >= x.StartDate.Date
                                                       && DateTime.Now.Date <= x.EndDate.Date
                                                       && string.IsNullOrEmpty(x.DeleterUsername))
                                                     .OrderBy(x => x.EndDate)
                                                     .Select(x => x.Id).ToList();
        }

        public SalesIncentivePrograms GetById(Guid id)
        {
            return _salesIncentiveProgramRepository.GetAll().Include(x => x.SalesIncentiveProgramAttachments).FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var salesIncentiveProgram = _salesIncentiveProgramRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _salesIncentiveProgramRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            salesIncentiveProgram.DeleterUsername = username;
            salesIncentiveProgram.DeletionTime = DateTime.Now;
            _salesIncentiveProgramRepository.Update(salesIncentiveProgram);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Sales Incentive Program", id, salesIncentiveProgram.Title, LogAction.Delete.ToString(), oldObject, salesIncentiveProgram);
        }

        public void Update(SalesIncentivePrograms input)
        {
            var oldObject = _salesIncentiveProgramRepository.GetAll().AsNoTracking().Include(x => x.SalesIncentiveProgramAttachments).FirstOrDefault(x => x.Id == input.Id);
            _salesIncentiveProgramRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Sales Incentive Program", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public SalesIncentiveDashboardDto Dashboard(Guid SalesIncentiveId)
        {
            var SalesIncentive = _salesIncentiveProgramRepository.GetAll()
                        .Where(x => x.Id == SalesIncentiveId && string.IsNullOrEmpty(x.DeleterUsername))
                        .Include(x => x.SalesIncentiveProgramTarget)
                        .Include(x => x.ProductTypes)
                        .FirstOrDefault();

            var targets = new List<SalesIncentiveTargetDashboardDto>();
            foreach (var detail in SalesIncentive.SalesIncentiveProgramTarget)
            {
                var _target = new SalesIncentiveTargetDashboardDto
                {
                    DealerName = detail.DealerName,
                    Kota = detail.Kota,
                    Karesidenan = detail.Karesidenan,
                    Target = detail.Target,
                    Transaksi = detail.Transaksi,
                    Persentase = (detail.Transaksi)/detail.Target
                };
                targets.Add(_target);
            }

            var output = new SalesIncentiveDashboardDto
            {
                StartDate = SalesIncentive.StartDate,
                EndDate = SalesIncentive.EndDate,
                ProductType = SalesIncentive.ProductTypes.ProductName,
                TipePembayaran = SalesIncentive.TipePembayaran.Value,
                Incentive = SalesIncentive.Incentive,
                PotensiAchievement = (SalesIncentive.SalesIncentiveProgramTarget.First().Transaksi) * SalesIncentive.Incentive,
                target = targets
            };

            return output;
        }
    }
}
