using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ProgramAppService : FLPAppServiceBase, IProgramAppService
    {
        private readonly IRepository<Programs, Guid> _programRepository;

        public ProgramAppService(IRepository<Programs, Guid> programRepository)
        {
            _programRepository = programRepository;
        }

        public IQueryable<Programs> GetAll()
        {
            return _programRepository.GetAll().Include(y => y.ProgramAttachments);
        }

        public ICollection<ProgramAttachments> GetAllAttachments(Guid id)
        {
            var programs = _programRepository.GetAll().Include(x => x.ProgramAttachments);
            var attachments = programs.FirstOrDefault(x => x.Id == id).ProgramAttachments;
            return attachments;
        }

        public List<Guid> GetAllIds(string channel)
        {
            var program = _programRepository.GetAll().Where(x => x.IsPublished
                                                              && DateTime.Now.Date >= x.StartDate.Date
                                                              && DateTime.Now.Date <= x.EndDate.Date
                                                              && string.IsNullOrEmpty(x.DeleterUsername))
                                                    .OrderByDescending(x => x.EndDate);

            switch (channel)
            {
                case "H1":
                    return program.Where(x => x.H2).Select(x => x.Id).ToList();
                case "H2":
                    return program.Where(x => x.H2).Select(x => x.Id).ToList();
                case "H3":
                    return program.Where(x => x.H3).Select(x => x.Id).ToList();
                default:
                    return program.Select(x => x.Id).ToList();
            }

        }

        public Programs GetById(Guid id)
        {
            var programs = _programRepository.GetAll().Include(x => x.ProgramAttachments).FirstOrDefault(x => x.Id == id);
            return programs;
        }

        public void Create(Programs input)
        {
            _programRepository.Insert(input);
        }

        public void Update(Programs input)
        {
            _programRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var program = _programRepository.FirstOrDefault(x => x.Id == id);
            program.DeleterUsername = username;
            program.DeletionTime = DateTime.Now;
            _programRepository.Update(program);
        }
    }
}
