using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class AgendaAppService : FLPAppServiceBase, IAgendaAppService
    {
        private readonly IRepository<Agendas, Guid> _agendaRepository;
        private readonly IRepository<AgendaAssignments, Guid> _agendaAssignmentRepository;

        public AgendaAppService(IRepository<Agendas, Guid> agendaRepository,
                                         IRepository<AgendaAssignments, Guid> agendaAssignmentRepository)
        {
            _agendaRepository = agendaRepository;
            _agendaAssignmentRepository = agendaAssignmentRepository;
        }

        public ServiceResult Assign(AgendaAssignDto input)
        {
            try 
            { 
                foreach (var idmpm in input.Assignments)
                {
                    AgendaAssignments assignment = new AgendaAssignments();
                    assignment.Id = Guid.NewGuid();
                    assignment.CreatorUsername = input.CreatorUsername;
                    assignment.CreationTime = DateTime.UtcNow.AddHours(7);
                    assignment.AgendaId = input.AgendaId;
                    assignment.IDMPM = idmpm;

                    _agendaAssignmentRepository.Insert(assignment);
                }
                return new ServiceResult() { IsSuccess = true, Message = "Insert Data Success" };
            }
            catch (Exception)
            { return new ServiceResult() { IsSuccess = false, Message = "Insert Data Failed" }; }

        }

        public async Task<ServiceResult> Create([FromForm]AgendaCreateDto input)
        {
            try
            {
                var id = Guid.NewGuid();
                Agendas agenda = new Agendas();

                agenda.Id = id;
                agenda.Name = input.Name;
                agenda.Description = input.Description;
                agenda.ComingDate = input.ComingDate;
                agenda.StorageUrl = input.file != null ? await AppHelpers.InsertAndGetUrlAzure(input.file, "IMG_" + id, "agenda") : string.Empty;
                agenda.CreatorUsername = input.CreatorUsername;
                agenda.CreationTime = DateTime.UtcNow.AddHours(7);
                agenda.AgendaAssignments = new List<AgendaAssignments>();
                string[] ids = input.Assignments[0].Split(',');

                foreach (var idmpm in ids)
                {
                    AgendaAssignments assignments = new AgendaAssignments();
                    assignments.Id = Guid.NewGuid();
                    var str = new string((from c in idmpm where char.IsDigit(c) select c).ToArray());
                    assignments.IDMPM = int.Parse(str);
                    assignments.AgendaId = agenda.Id;
                    assignments.CreatorUsername = input.CreatorUsername;
                    assignments.CreationTime = DateTime.UtcNow.AddHours(7);
                    agenda.AgendaAssignments.Add(assignments);
                }

                _agendaRepository.Insert(agenda);

                return new ServiceResult() { IsSuccess = true, Message = "Insert Data Success" };
            }
            catch (Exception e)
            { return new ServiceResult() { IsSuccess = false, Message = e.Message }; }

        }

        public List<Agendas> GetAgendaByUser(int idmpm)
        {
            List<Guid> eventIds = _agendaAssignmentRepository.GetAll().Where(x => x.IDMPM == idmpm).Select(x => x.AgendaId).ToList();

            return _agendaRepository.GetAllIncluding(x => x.AgendaAssignments).Where(x => eventIds.Contains(x.Id)).ToList();
        }

        

        public ServiceResult UnAssign(AgendaDeleteDto input)
        {
            try
            {
                foreach (var idmpm in input.IDMPM)
                {
                    var assignment = _agendaAssignmentRepository.GetAll().Where(x => x.IDMPM == idmpm && x.AgendaId == input.AgendaId).FirstOrDefault();
                    _agendaAssignmentRepository.Delete(assignment);
                }
                return new ServiceResult() { IsSuccess = true, Message = "Update Data Success" };
            }
            catch (Exception)
            { return new ServiceResult() { IsSuccess = false, Message = "Update Data Failed" }; }
        }
    }
}
